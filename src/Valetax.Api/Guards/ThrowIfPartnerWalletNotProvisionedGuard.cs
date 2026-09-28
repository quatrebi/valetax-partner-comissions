using Valetax.Api.Exceptions;

namespace Valetax.Api.Guards;

public static class ThrowIfPartnerWalletNotProvisionedGuard
{
    public static void ThrowIfPartnerWalletNotProvisioned(Guid partnerExternalId, bool walletExists)
    {
        if (!walletExists)
            throw new PartnerWalletNotProvisionedException(partnerExternalId);
    }
}