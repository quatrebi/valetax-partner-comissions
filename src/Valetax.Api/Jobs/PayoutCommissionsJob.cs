using Quartz;
using Valetax.Api.Infrastructure;

namespace Valetax.Api.Jobs;

[DisallowConcurrentExecution]
public sealed class PayoutCommissionsJob(
    ICommissionPayoutService payoutService,
    CommissionsMetrics metrics) : IJob
{
    public async ValueTask Execute(IJobExecutionContext context, CancellationToken ct = default)
    {
        metrics.OldestPendingProfitEvent(await payoutService.GetOldestPendingCreatedAtAsync(ct));

        var eventIds = await payoutService.ClaimAsync(ct);

        foreach (var eventId in eventIds)
        {
            await payoutService.PayAsync(eventId, ct);
        }
    }
}