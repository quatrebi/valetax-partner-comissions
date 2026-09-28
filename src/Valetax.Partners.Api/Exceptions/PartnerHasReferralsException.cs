using Valetax.Infrastructure.Exceptions;

namespace Valetax.Partners.Api.Exceptions;

public sealed class PartnerHasReferralsException(Guid partnerExternalId)
    : ValetaxException($"Partner '{partnerExternalId}' has referrals and cannot be deleted.", StatusCodes.Status409Conflict);