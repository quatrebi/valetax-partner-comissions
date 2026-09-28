using System.Globalization;
using Grpc.Core;
using Valetax.Wallets.Api.Grpc;

namespace Valetax.Api.Infrastructure;

public sealed class WalletsClient(
    WalletsService.WalletsServiceClient walletsClient,
    WalletPayoutsService.WalletPayoutsServiceClient payoutsClient) : IWalletsClient
{
    private static readonly TimeSpan CallTimeout = TimeSpan.FromSeconds(5);

    public async Task<bool> WalletExistsAsync(Guid ownerId, CancellationToken ct)
    {
        try
        {
            await walletsClient.GetWalletAsync(
                new GetWalletRequest { OwnerId = ownerId.ToString() },
                deadline: DateTime.UtcNow.Add(CallTimeout),
                cancellationToken: ct);

            return true;
        }
        catch (RpcException exception) when (exception.StatusCode == StatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task<bool> PayoutCommissionAsync(Guid ownerId, Guid commissionId, decimal amount, CancellationToken ct)
    {
        var response = await payoutsClient.PayoutCommissionAsync(
            new PayoutCommissionRequest
            {
                OwnerId = ownerId.ToString(),
                CommissionId = commissionId.ToString(),
                Amount = amount.ToString(CultureInfo.InvariantCulture)
            },
            deadline: DateTime.UtcNow.Add(CallTimeout),
            cancellationToken: ct);

        return response.AlreadyPaid;
    }
}