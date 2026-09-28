using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Valetax.Infrastructure.Endpoints;
using Valetax.Partners.Api.Guards;
using Valetax.Partners.Api.Persistence;

namespace Valetax.Partners.Api.Features.DeletePartner;

public sealed class DeletePartnerEndpoint : IApiEndpoint
{
    public void UseMetadata(IEndpointRouteBuilder routeBuilder) => routeBuilder
        .MapGroup(Constants.Api.PartnersGroup)
        .MapDelete("/{id:guid}", Handler);

    private static async Task<IResult> Handler(
        [FromRoute] Guid id,
        [FromServices] IPartnersDbContext dbContext,
        CancellationToken ct = default)
    {
        var partner = await dbContext.Partners.FirstOrDefaultAsync(x => x.ExternalId == id, ct);

        if (partner is null)
            return Results.NotFound();

        var hasReferrals = await dbContext.Partners.AnyAsync(x => x.RefPartnerId == id, ct);
        ThrowIfPartnerHasReferralsGuard.ThrowIfPartnerHasReferrals(id, hasReferrals);

        dbContext.Partners.Remove(partner);
        await dbContext.SaveChangesAsync(ct);

        return Results.NoContent();
    }
}