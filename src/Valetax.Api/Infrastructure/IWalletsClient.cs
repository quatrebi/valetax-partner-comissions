namespace Valetax.Api.Infrastructure;

public interface IWalletsClient
{
    Task<bool> WalletExistsAsync(Guid ownerId, CancellationToken ct);

    Task<bool> PayoutCommissionAsync(Guid ownerId, Guid commissionId, decimal amount, CancellationToken ct);
}