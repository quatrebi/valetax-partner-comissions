namespace Valetax.Partners.Api.Contracts.IntegrationEvents;

public sealed record PartnerCreatedIntegrationEvent(Guid PartnerExternalId, DateTimeOffset CreatedAt)
{
    public const string Topic = "partners.partner-created";
}