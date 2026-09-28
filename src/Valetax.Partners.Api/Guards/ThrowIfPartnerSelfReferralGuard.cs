using Valetax.Partners.Api.Exceptions;

namespace Valetax.Partners.Api.Guards;

public static class ThrowIfPartnerSelfReferralGuard
{
    public static void ThrowIfPartnerSelfReferral(Guid partnerExternalId, Guid? refPartnerExternalId)
    {
        if (partnerExternalId == refPartnerExternalId)
            throw new PartnerSelfReferralException(partnerExternalId);
    }
}