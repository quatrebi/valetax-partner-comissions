namespace Valetax.Api.Exceptions;

public sealed class InvalidCommissionLevelException(int level)
    : InvalidOperationException($"Commission level '{level}' must be greater than zero.");