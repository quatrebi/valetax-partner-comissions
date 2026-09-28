namespace Valetax.Api.Features.CreateProfitEvent;

public sealed record CreateProfitEventDto(Guid ExternalId, Guid PartnerExternalId, decimal Profit);