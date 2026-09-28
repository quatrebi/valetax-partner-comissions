using Valetax.Partners.Api.Guards;

namespace Valetax.Partners.Api.Domain;

public sealed class Partner
{
    public Guid ExternalId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? RefPartnerId { get; private set; }

    public static Partner Create(Guid externalId, Guid? refPartnerExternalId = null)
    {
        var partner = new Partner
        {
            ExternalId = externalId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        partner.SetRefPartner(refPartnerExternalId);

        return partner;
    }

    public void SetRefPartner(Guid? refPartnerExternalId)
    {
        ThrowIfPartnerSelfReferralGuard.ThrowIfPartnerSelfReferral(ExternalId, refPartnerExternalId);
        RefPartnerId = refPartnerExternalId;
    }
}