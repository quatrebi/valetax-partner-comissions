namespace Valetax.Infrastructure.Exceptions;

public sealed class InvalidConfigurationException(string key)
    : InvalidOperationException($"Configuration value '{key}' is required.");