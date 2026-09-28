using Valetax.Partners.Api.Domain;
using Valetax.Partners.Api.Exceptions;

namespace Valetax.Partners.Api.Guards;

public static class ThrowIfPartnerAlreadyExistsGuard
{
    public static void ThrowIfPartnerAlreadyExists(Partner existing, Guid? refPartnerExternalId)
    {
        if (existing.RefPartnerId != refPartnerExternalId)
            throw new PartnerAlreadyExistsException(existing.ExternalId);
    }
}