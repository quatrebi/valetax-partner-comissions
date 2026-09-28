using System.Diagnostics.CodeAnalysis;
using Valetax.Api.Exceptions;
using Valetax.Api.Infrastructure;

namespace Valetax.Api.Guards;

public static class ThrowIfPartnerNotFoundGuard
{
    public static void ThrowIfPartnerNotFound(Guid partnerExternalId, [NotNull] PartnerChain? partnerChain)
    {
        if (partnerChain is null)
            throw new PartnerNotFoundException(partnerExternalId);
    }
}