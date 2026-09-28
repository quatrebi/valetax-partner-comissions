using Valetax.Api.Domain;
using Valetax.Api.Exceptions;

namespace Valetax.Api.Guards;

public static class ThrowIfProfitEventAlreadyExistsGuard
{
    public static void ThrowIfProfitEventAlreadyExists(ProfitEvent existing, Guid partnerExternalId, decimal profit)
    {
        if (existing.PartnerExternalId != partnerExternalId || existing.Profit != profit)
            throw new ProfitEventAlreadyExistsException(existing.ExternalId);
    }
}