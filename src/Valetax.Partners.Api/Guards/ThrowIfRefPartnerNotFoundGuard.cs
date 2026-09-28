using Valetax.Partners.Api.Exceptions;

namespace Valetax.Partners.Api.Guards;

public static class ThrowIfRefPartnerNotFoundGuard
{
    public static void ThrowIfRefPartnerNotFound(Guid refPartnerExternalId, bool refPartnerExists)
    {
        if (!refPartnerExists)
            throw new RefPartnerNotFoundException(refPartnerExternalId);
    }
}