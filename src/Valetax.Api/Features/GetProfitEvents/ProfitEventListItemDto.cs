using Valetax.Api.Domain;

namespace Valetax.Api.Features.GetProfitEvents;

public sealed record ProfitEventListItemDto(
    Guid ExternalId,
    Guid PartnerExternalId,
    decimal Profit,
    CommissionSchemeType SchemaType,
    ProfitEventStatus Status,
    DateTimeOffset CreatedAt);