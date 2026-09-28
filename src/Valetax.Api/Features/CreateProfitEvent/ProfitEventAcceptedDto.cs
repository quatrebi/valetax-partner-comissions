using Valetax.Api.Domain;

namespace Valetax.Api.Features.CreateProfitEvent;

public sealed record ProfitEventAcceptedDto(Guid ExternalId, CommissionSchemeType SchemaType, ProfitEventStatus Status)
{
    public static ProfitEventAcceptedDto From(ProfitEvent profitEvent) => new(
        profitEvent.ExternalId,
        profitEvent.SchemaType,
        profitEvent.Status);
}