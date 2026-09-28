using Valetax.Infrastructure.Exceptions;

namespace Valetax.Partners.Api.Exceptions;

public sealed class PartnerReferralCycleException(Guid partnerExternalId, Guid refPartnerExternalId)
    : ValetaxException($"Partner '{refPartnerExternalId}' is a referral of '{partnerExternalId}' and cannot be their referrer.");