using System.Diagnostics.CodeAnalysis;
using Valetax.Infrastructure.Exceptions;

namespace Valetax.Infrastructure.Guards;

public static class ThrowIfInvalidConfigurationGuard
{
    public static void ThrowIfInvalidConfiguration([NotNull] string? value, string key)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidConfigurationException(key);
    }
}