using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Valetax.Wallets.Api.Exceptions;
using Valetax.Wallets.Api.Grpc;
using Valetax.Wallets.Api.Persistence;

namespace Valetax.Wallets.Api.Features.GetWallet;

public sealed class GetWalletGrpcService(IWalletsDbContext dbContext) : WalletsService.WalletsServiceBase
{
    public override async Task<GetWalletResponse> GetWallet(
        GetWalletRequest request,
        ServerCallContext ctx)
    {
        if (!Guid.TryParse(request.OwnerId, out var ownerId))
            throw new InvalidOwnerIdException(request.OwnerId);

        if (!await dbContext.Wallets.AnyAsync(x => x.OwnerId == ownerId, ctx.CancellationToken))
            throw new WalletNotFoundException(ownerId);

        return new GetWalletResponse { OwnerId = request.OwnerId };
    }
}