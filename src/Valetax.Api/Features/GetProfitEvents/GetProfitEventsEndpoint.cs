using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Valetax.Api.Persistence;
using Valetax.Infrastructure.Endpoints;
using Valetax.Infrastructure.Pagination;

namespace Valetax.Api.Features.GetProfitEvents;

public sealed class GetProfitEventsEndpoint : IApiEndpoint<PageDto>
{
    public void UseMetadata(IEndpointRouteBuilder routeBuilder) => routeBuilder
        .MapGroup(Constants.Api.EventsGroup)
        .MapGet("", Handler);

    private static async Task<IResult> Handler(
        [FromQuery] Guid partnerExternalId,
        [AsParameters] PageDto page,
        [FromServices] ICommissionsDbContext dbContext,
        CancellationToken ct = default)
    {
        var profitEvents = await dbContext.ProfitEvents.AsNoTracking()
            .Where(x => x.PartnerExternalId == partnerExternalId)
            .OrderByDescending(x => x.CreatedAt)
            .ThenBy(x => x.ExternalId)
            .Paginate(page)
            .Select(x => new ProfitEventListItemDto(
                x.ExternalId,
                x.PartnerExternalId,
                x.Profit,
                x.SchemaType,
                x.Status,
                x.CreatedAt))
            .ToListAsync(ct);

        return Results.Ok(profitEvents);
    }
}