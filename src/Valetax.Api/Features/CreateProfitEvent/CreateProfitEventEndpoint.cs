using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Valetax.Api.Domain;
using Valetax.Api.Guards;
using Valetax.Api.Infrastructure;
using Valetax.Api.Persistence;
using Valetax.Api.Services;
using Valetax.Infrastructure.Endpoints;
using Valetax.Infrastructure.Persistence;

namespace Valetax.Api.Features.CreateProfitEvent;

public sealed class CreateProfitEventEndpoint : IApiEndpoint<CreateProfitEventDto>
{
    public void UseMetadata(IEndpointRouteBuilder routeBuilder) => routeBuilder
        .MapGroup(Constants.Api.EventsGroup)
        .MapPost("", Handler);

    private static async Task<IResult> Handler(
        [FromBody] CreateProfitEventDto dto,
        [FromServices] ICommissionsDbContext dbContext,
        [FromServices] IPartnersClient partnersClient,
        [FromServices] IWalletsClient walletsClient,
        [FromServices] ICommissionService commissionService,
        [FromServices] CommissionsMetrics metrics,
        CancellationToken ct = default)
    {
        var existing = await dbContext.ProfitEvents.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ExternalId == dto.ExternalId, ct);

        if (existing is not null)
            return ExistingResult(existing, dto);

        var partnerChain = await partnersClient.GetPartnerChainAsync(dto.PartnerExternalId, ct);
        ThrowIfPartnerNotFoundGuard.ThrowIfPartnerNotFound(dto.PartnerExternalId, partnerChain);

        var walletExists = await walletsClient.WalletExistsAsync(dto.PartnerExternalId, ct);
        ThrowIfPartnerWalletNotProvisionedGuard.ThrowIfPartnerWalletNotProvisioned(dto.PartnerExternalId, walletExists);

        var schemaType = await dbContext.CommissionSettings.AsNoTracking()
            .Select(x => x.SchemaType)
            .SingleAsync(ct);

        var profitEvent = ProfitEvent.Create(
            dto.ExternalId,
            dto.PartnerExternalId,
            dto.Profit,
            schemaType);

        profitEvent.SetCommissions(partnerChain.RefPartnerIds.Select((refPartnerId, index) => Commission.Create(
            profitEvent.ExternalId,
            refPartnerId,
            index + 1,
            commissionService.Calculate(profitEvent.Profit, index + 1, profitEvent.SchemaType),
            profitEvent.SchemaType)));

        await dbContext.ProfitEvents.AddAsync(profitEvent, ct);
        try
        {
            await dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateException exception) when (exception.IsUniqueViolation)
        {
            existing = await dbContext.ProfitEvents.AsNoTracking()
                .SingleAsync(x => x.ExternalId == dto.ExternalId, ct);

            return ExistingResult(existing, dto);
        }

        metrics.ProfitEventAccepted(profitEvent.Commissions.Count);

        return AcceptedResult(profitEvent);
    }

    private static IResult ExistingResult(ProfitEvent profitEvent, CreateProfitEventDto dto)
    {
        ThrowIfProfitEventAlreadyExistsGuard.ThrowIfProfitEventAlreadyExists(
            profitEvent,
            dto.PartnerExternalId,
            dto.Profit);

        return AcceptedResult(profitEvent);
    }

    private static IResult AcceptedResult(ProfitEvent profitEvent) => Results.Accepted(
        $"{Constants.Api.EventsGroup}/{profitEvent.ExternalId}",
        ProfitEventAcceptedDto.From(profitEvent));
}