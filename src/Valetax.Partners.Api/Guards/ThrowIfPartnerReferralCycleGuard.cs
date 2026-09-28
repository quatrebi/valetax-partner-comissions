using Valetax.Partners.Api.Exceptions;

namespace Valetax.Partners.Api.Guards;

public static class ThrowIfPartnerReferralCycleGuard
{
    public static void ThrowIfPartnerReferralCycle(
        Guid partnerExternalId,
        Guid refPartnerExternalId,
        IEnumerable<Guid> refPartnerAncestorIds)
    {
        if (refPartnerAncestorIds.Contains(partnerExternalId))
            throw new PartnerReferralCycleException(partnerExternalId, refPartnerExternalId);
    }
}