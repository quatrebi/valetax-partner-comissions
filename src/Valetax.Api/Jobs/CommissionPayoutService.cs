using System.Diagnostics;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Valetax.Api.Domain;
using Valetax.Api.Infrastructure;
using Valetax.Api.Persistence;

namespace Valetax.Api.Jobs;

public sealed class CommissionPayoutService(
    ICommissionsDbContext dbContext,
    IWalletsClient walletsClient,
    CommissionsMetrics metrics,
    IOptions<CommissionPayoutOptions> options,
    ILogger<CommissionPayoutService> logger) : ICommissionPayoutService
{
    private static readonly ActivitySource ActivitySource = new(CommissionsMetrics.MeterName);

    private CommissionPayoutOptions Options => options.Value;

    public Task<List<Guid>> ClaimAsync(CancellationToken ct) => dbContext.Database
        .SqlQuery<Guid>($"""
            UPDATE "ProfitEvents"
            SET "NextAttemptAt" = now() + {Options.LeaseDuration}
            WHERE "ExternalId" IN (
                SELECT "ExternalId"
                FROM "ProfitEvents"
                WHERE "Status" = {(int)ProfitEventStatus.Pending} AND "NextAttemptAt" <= now()
                ORDER BY "NextAttemptAt"
                LIMIT {Options.BatchSize}
                FOR UPDATE SKIP LOCKED
            )
            RETURNING "ExternalId" AS "Value"
            """)
        .ToListAsync(ct);

    public Task<DateTimeOffset?> GetOldestPendingCreatedAtAsync(CancellationToken ct) => dbContext.ProfitEvents
        .Where(x => x.Status == ProfitEventStatus.Pending)
        .MinAsync(x => (DateTimeOffset?)x.CreatedAt, ct);

    public async Task PayAsync(Guid eventId, CancellationToken ct)
    {
        using var activity = ActivitySource.StartActivity("PayProfitEventCommissions");
        activity?.SetTag("valetax.profit_event.id", eventId);

        try
        {
            await PayCommissionsAsync(eventId, ct);
        }
        catch (Exception exception) when (!ct.IsCancellationRequested)
        {
            // A failed event must not leave tracked changes for the next one.
            dbContext.ChangeTracker.Clear();

            logger.LogError(exception, "Failed to pay commissions of profit event {EventId}", eventId);
            activity?.SetStatus(ActivityStatusCode.Error);
        }
    }

    private async Task PayCommissionsAsync(Guid eventId, CancellationToken ct)
    {
        var profitEvent = await dbContext.ProfitEvents
            .Include(x => x.Commissions)
            .SingleAsync(x => x.ExternalId == eventId, ct);

        foreach (var commission in profitEvent.Commissions
                     .Where(x => x.PaymentStatus == CommissionPaymentStatus.Pending)
                     .OrderBy(x => x.Level))
        {
            try
            {
                var alreadyPaid = await walletsClient.PayoutCommissionAsync(
                    commission.RecipientExternalId,
                    commission.Id,
                    commission.Amount,
                    ct);

                commission.MarkPaid();

                if (!alreadyPaid)
                    metrics.CommissionPaid();
            }
            catch (RpcException exception) when (!ct.IsCancellationRequested)
            {
                metrics.CommissionPayoutFailed(exception.StatusCode.ToString());
                logger.LogWarning(exception,
                    "Payout failed for commission {CommissionId} of profit event {EventId}",
                    commission.Id, eventId);
            }
        }

        if (profitEvent.Commissions.All(x => x.PaymentStatus == CommissionPaymentStatus.Paid))
            profitEvent.Complete();
        else
            profitEvent.RetryAt(DateTimeOffset.UtcNow.Add(Options.RetryDelay));

        await dbContext.SaveChangesAsync(ct);
    }
}