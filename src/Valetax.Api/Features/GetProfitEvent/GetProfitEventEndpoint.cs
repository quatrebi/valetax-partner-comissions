using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Valetax.Api.Persistence;
using Valetax.Infrastructure.Endpoints;

namespace Valetax.Api.Features.GetProfitEvent;

public sealed class GetProfitEventEndpoint : IApiEndpoint
{
    public void UseMetadata(IEndpointRouteBuilder routeBuilder) => routeBuilder
        .MapGroup(Constants.Api.EventsGroup)
        .MapGet("/{id:guid}", Handler);

    private static async Task<IResult> Handler(
        [FromRoute] Guid id,
        [FromServices] ICommissionsDbContext dbContext,
        CancellationToken ct = default)
    {
        var profitEvent = await dbContext.ProfitEvents.AsNoTracking()
            .Include(x => x.Commissions)
            .FirstOrDefaultAsync(x => x.ExternalId == id, ct);

        return profitEvent is null
            ? Results.NotFound()
            : Results.Ok(GetProfitEventDto.From(profitEvent));
    }
}