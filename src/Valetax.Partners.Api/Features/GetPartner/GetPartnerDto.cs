using Valetax.Partners.Api.Domain;

namespace Valetax.Partners.Api.Features.GetPartner;

public sealed record GetPartnerDto(
    Guid ExternalId,
    Guid? RefPartnerExternalId,
    DateTimeOffset CreatedAt)
{
    public static GetPartnerDto From(Partner partner) => new(
        partner.ExternalId,
        partner.RefPartnerId,
        partner.CreatedAt);
}