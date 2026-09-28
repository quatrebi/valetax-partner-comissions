using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Valetax.Infrastructure.Endpoints;
using Valetax.Partners.Api.Persistence;

namespace Valetax.Partners.Api.Features.GetPartner;

public sealed class GetPartnerEndpoint : IApiEndpoint
{
    public void UseMetadata(IEndpointRouteBuilder routeBuilder) => routeBuilder
        .MapGroup(Constants.Api.PartnersGroup)
        .MapGet("/{id:guid}", Handler);

    private static async Task<IResult> Handler(
        [FromRoute] Guid id,
        [FromServices] IPartnersDbContext dbContext,
        CancellationToken ct = default)
    {
        var partner = await dbContext.Partners
            .AsNoTracking()
            .FirstOrDefaultAsync(partner => partner.ExternalId == id, ct);

        return partner is null
            ? Results.NotFound()
            : Results.Ok(GetPartnerDto.From(partner));
    }
}