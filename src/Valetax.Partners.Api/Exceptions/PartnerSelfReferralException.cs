using Valetax.Infrastructure.Exceptions;

namespace Valetax.Partners.Api.Exceptions;

public sealed class PartnerSelfReferralException(Guid partnerExternalId)
    : ValetaxException($"Partner '{partnerExternalId}' cannot be their own referrer.");