using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Valetax.Infrastructure.Endpoints;
using Valetax.Infrastructure.Pagination;
using Valetax.Partners.Api.Features.GetPartner;
using Valetax.Partners.Api.Persistence;

namespace Valetax.Partners.Api.Features.GetPartners;

public sealed class GetPartnersEndpoint : IApiEndpoint<PageDto>
{
    public void UseMetadata(IEndpointRouteBuilder routeBuilder) => routeBuilder
        .MapGroup(Constants.Api.PartnersGroup)
        .MapGet("", Handler);

    private static async Task<IResult> Handler(
        [AsParameters] PageDto page,
        [FromServices] IPartnersDbContext dbContext,
        CancellationToken ct = default)
    {
        var partners = await dbContext.Partners
            .AsNoTracking()
            .OrderBy(partner => partner.CreatedAt)
            .ThenBy(partner => partner.ExternalId)
            .Paginate(page)
            .Select(partner => new GetPartnerDto(
                partner.ExternalId,
                partner.RefPartnerId,
                partner.CreatedAt))
            .ToListAsync(ct);

        return Results.Ok(partners);
    }
}