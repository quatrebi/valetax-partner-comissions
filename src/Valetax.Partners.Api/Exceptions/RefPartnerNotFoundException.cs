using Valetax.Infrastructure.Exceptions;

namespace Valetax.Partners.Api.Exceptions;

public sealed class RefPartnerNotFoundException(Guid refPartnerExternalId)
    : ValetaxException($"Referrer '{refPartnerExternalId}' not found.");