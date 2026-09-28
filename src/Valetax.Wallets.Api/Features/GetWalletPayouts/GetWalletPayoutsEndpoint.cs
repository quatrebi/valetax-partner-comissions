using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Valetax.Infrastructure.Endpoints;
using Valetax.Infrastructure.Pagination;
using Valetax.Wallets.Api.Persistence;

namespace Valetax.Wallets.Api.Features.GetWalletPayouts;

public sealed class GetWalletPayoutsEndpoint : IApiEndpoint<PageDto>
{
    public void UseMetadata(IEndpointRouteBuilder routeBuilder) => routeBuilder
        .MapGroup(Constants.Api.WalletsGroup)
        .MapGet("/{ownerId:guid}/payouts", Handler);

    private static async Task<IResult> Handler(
        [FromRoute] Guid ownerId,
        [AsParameters] PageDto page,
        [FromServices] IWalletsDbContext dbContext,
        CancellationToken ct = default)
    {
        if (!await dbContext.Wallets.AnyAsync(x => x.OwnerId == ownerId, ct))
            return Results.NotFound();

        var payouts = dbContext.Payouts
            .AsNoTracking()
            .Where(x => x.WalletId == ownerId);

        var balance = await payouts.SumAsync(x => x.Amount, ct);
        var items = await payouts
            .OrderByDescending(x => x.PaidAt)
            .ThenBy(x => x.CommissionId)
            .Paginate(page)
            .Select(x => new WalletPayoutDto(x.CommissionId, x.Amount, x.PaidAt))
            .ToListAsync(ct);

        return Results.Ok(new GetWalletPayoutsDto(ownerId, balance, items));
    }
}