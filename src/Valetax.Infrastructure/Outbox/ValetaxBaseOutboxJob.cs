using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace Valetax.Infrastructure.Outbox;

[DisallowConcurrentExecution]
public abstract class ValetaxBaseOutboxJob(
    IOutboxDbContext dbContext,
    IOptions<OutboxOptions> options,
    ILogger logger) : IJob
{
    private OutboxOptions Options => options.Value;

    public async ValueTask Execute(IJobExecutionContext context, CancellationToken ct = default)
    {
        var messageIds = await ClaimAsync(ct);

        if (messageIds.Count == 0)
            return;

        var messages = await dbContext.OutboxMessages.AsNoTracking()
            .Where(x => messageIds.Contains(x.Id))
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(ct);

        foreach (var message in messages)
        {
            try
            {
                using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
                timeout.CancelAfter(Options.PublishTimeout);

                await PublishAsync(message, timeout.Token);

                await dbContext.OutboxMessages
                    .Where(x => x.Id == message.Id)
                    .ExecuteDeleteAsync(ct);
            }
            catch (Exception exception) when (!ct.IsCancellationRequested)
            {
                logger.LogWarning(exception,
                    "Failed to publish outbox message {MessageId} to {Topic}", message.Id, message.Topic);

                await dbContext.OutboxMessages
                    .Where(x => x.Id == message.Id)
                    .ExecuteUpdateAsync(updates => updates
                        .SetProperty(x => x.NextAttemptAt, DateTimeOffset.UtcNow.Add(Options.RetryDelay)), ct);
            }
        }
    }

    protected abstract Task PublishAsync(OutboxMessage message, CancellationToken ct);

    // Leases a batch: a claimed message is invisible to other instances until "NextAttemptAt".
    private Task<List<Guid>> ClaimAsync(CancellationToken ct) => dbContext.Database
        .SqlQuery<Guid>($"""
            UPDATE "OutboxMessages"
            SET "NextAttemptAt" = now() + {Options.LeaseDuration}
            WHERE "Id" IN (
                SELECT "Id"
                FROM "OutboxMessages"
                WHERE "NextAttemptAt" <= now()
                ORDER BY "NextAttemptAt"
                LIMIT {Options.BatchSize}
                FOR UPDATE SKIP LOCKED
            )
            RETURNING "Id" AS "Value"
            """)
        .ToListAsync(ct);
}