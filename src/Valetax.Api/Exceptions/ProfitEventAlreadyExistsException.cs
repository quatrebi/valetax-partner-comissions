using Valetax.Infrastructure.Exceptions;

namespace Valetax.Api.Exceptions;

public sealed class ProfitEventAlreadyExistsException(Guid profitEventExternalId)
    : ValetaxException(
        $"Profit event '{profitEventExternalId}' already exists with different data.",
        StatusCodes.Status409Conflict);