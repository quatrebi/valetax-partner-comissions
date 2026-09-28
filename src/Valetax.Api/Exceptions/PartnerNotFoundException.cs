using Valetax.Infrastructure.Exceptions;

namespace Valetax.Api.Exceptions;

public sealed class PartnerNotFoundException(Guid partnerExternalId)
    : ValetaxException($"Partner '{partnerExternalId}' not found.");