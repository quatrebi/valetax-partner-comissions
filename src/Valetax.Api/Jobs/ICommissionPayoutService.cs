namespace Valetax.Api.Jobs;

public interface ICommissionPayoutService
{
    Task<List<Guid>> ClaimAsync(CancellationToken ct);

    Task<DateTimeOffset?> GetOldestPendingCreatedAtAsync(CancellationToken ct);

    Task PayAsync(Guid eventId, CancellationToken ct);
}