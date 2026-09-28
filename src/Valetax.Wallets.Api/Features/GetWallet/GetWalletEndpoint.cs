using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Valetax.Infrastructure.Endpoints;
using Valetax.Wallets.Api.Persistence;

namespace Valetax.Wallets.Api.Features.GetWallet;

public sealed class GetWalletEndpoint : IApiEndpoint
{
    public void UseMetadata(IEndpointRouteBuilder routeBuilder) => routeBuilder
        .MapGroup(Constants.Api.WalletsGroup)
        .MapGet("/{ownerId:guid}", Handler);

    private static async Task<IResult> Handler(
        [FromRoute] Guid ownerId,
        [FromServices] IWalletsDbContext dbContext,
        CancellationToken ct = default)
    {
        if (!await dbContext.Wallets.AnyAsync(x => x.OwnerId == ownerId, ct))
            return Results.NotFound();

        var balance = await dbContext.Payouts
            .AsNoTracking()
            .Where(x => x.WalletId == ownerId)
            .SumAsync(x => x.Amount, ct);

        return Results.Ok(new GetWalletDto(ownerId, balance));
    }
}