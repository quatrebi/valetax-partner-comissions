using Valetax.Partners.Api.Exceptions;

namespace Valetax.Partners.Api.Guards;

public static class ThrowIfPartnerHasReferralsGuard
{
    public static void ThrowIfPartnerHasReferrals(Guid partnerExternalId, bool hasReferrals)
    {
        if (hasReferrals)
            throw new PartnerHasReferralsException(partnerExternalId);
    }
}