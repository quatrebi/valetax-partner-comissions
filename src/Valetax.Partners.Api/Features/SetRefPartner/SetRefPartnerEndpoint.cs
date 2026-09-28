using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Valetax.Infrastructure.Endpoints;
using Valetax.Partners.Api.Features.GetPartner;
using Valetax.Partners.Api.Features.GetPartnerTree;
using Valetax.Partners.Api.Persistence;

namespace Valetax.Partners.Api.Features.SetRefPartner;

public sealed class SetRefPartnerEndpoint : IApiEndpoint<SetRefPartnerDto>
{
    public void UseMetadata(IEndpointRouteBuilder routeBuilder) => routeBuilder
        .MapGroup(Constants.Api.PartnersGroup)
        .MapPut("/{id:guid}/ref-partner", Handler);

    private static async Task<IResult> Handler(
        [FromRoute] Guid id,
        [FromBody] SetRefPartnerDto dto,
        [FromServices] IPartnersDbContext dbContext,
        [FromServices] IPartnerTreeService treeService,
        CancellationToken ct = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
        await treeService.LockRefPartnerChangesAsync(ct);

        var partner = await dbContext.Partners.FirstOrDefaultAsync(x => x.ExternalId == id, ct);

        if (partner is null)
            return Results.NotFound();

        partner.SetRefPartner(dto.RefPartnerExternalId);
        await treeService.ThrowIfCannotSetRefPartnerAsync(partner.ExternalId, partner.RefPartnerId, ct);

        await dbContext.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Results.Ok(GetPartnerDto.From(partner));
    }
}