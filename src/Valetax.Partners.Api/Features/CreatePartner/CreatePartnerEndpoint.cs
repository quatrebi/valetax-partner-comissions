using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Valetax.Infrastructure.Endpoints;
using Valetax.Infrastructure.Outbox;
using Valetax.Infrastructure.Persistence;
using Valetax.Partners.Api.Contracts.IntegrationEvents;
using Valetax.Partners.Api.Domain;
using Valetax.Partners.Api.Features.GetPartner;
using Valetax.Partners.Api.Guards;
using Valetax.Partners.Api.Persistence;

namespace Valetax.Partners.Api.Features.CreatePartner;

public sealed class CreatePartnerEndpoint : IApiEndpoint<CreatePartnerDto>
{
    public void UseMetadata(IEndpointRouteBuilder routeBuilder) => routeBuilder
        .MapGroup(Constants.Api.PartnersGroup)
        .MapPost("", Handler);

    private static async Task<IResult> Handler(
        [FromBody] CreatePartnerDto dto,
        [FromServices] IPartnersDbContext dbContext,
        CancellationToken ct = default)
    {
        var existing = await dbContext.Partners.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ExternalId == dto.ExternalId, ct);

        if (existing is not null)
            return ExistingResult(existing, dto);

        var partner = Partner.Create(dto.ExternalId, dto.RefPartnerExternalId);

        // A new partner has no referrals yet, so it cannot close a cycle: only the referrer has to exist.
        if (partner.RefPartnerId is { } refPartnerId)
        {
            var refPartnerExists = await dbContext.Partners.AnyAsync(x => x.ExternalId == refPartnerId, ct);
            ThrowIfRefPartnerNotFoundGuard.ThrowIfRefPartnerNotFound(refPartnerId, refPartnerExists);
        }

        await dbContext.Partners.AddAsync(partner, ct);
        await dbContext.OutboxMessages.AddAsync(OutboxMessage.Create(
            PartnerCreatedIntegrationEvent.Topic,
            new PartnerCreatedIntegrationEvent(partner.ExternalId, partner.CreatedAt)), ct);

        try
        {
            await dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateException exception) when (exception.IsUniqueViolation)
        {
            existing = await dbContext.Partners.AsNoTracking()
                .SingleAsync(x => x.ExternalId == dto.ExternalId, ct);

            return ExistingResult(existing, dto);
        }

        return Results.Created(
            $"{Constants.Api.PartnersGroup}/{partner.ExternalId}",
            GetPartnerDto.From(partner));
    }

    private static IResult ExistingResult(Partner partner, CreatePartnerDto dto)
    {
        ThrowIfPartnerAlreadyExistsGuard.ThrowIfPartnerAlreadyExists(partner, dto.RefPartnerExternalId);

        return Results.Ok(GetPartnerDto.From(partner));
    }
}