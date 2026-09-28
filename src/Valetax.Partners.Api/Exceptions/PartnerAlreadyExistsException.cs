using Valetax.Infrastructure.Exceptions;

namespace Valetax.Partners.Api.Exceptions;

public sealed class PartnerAlreadyExistsException(Guid partnerExternalId)
    : ValetaxException($"Partner '{partnerExternalId}' already exists with different data.", StatusCodes.Status409Conflict);