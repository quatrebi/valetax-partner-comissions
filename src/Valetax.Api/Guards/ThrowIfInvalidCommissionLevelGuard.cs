using Valetax.Api.Exceptions;

namespace Valetax.Api.Guards;

public static class ThrowIfInvalidCommissionLevelGuard
{
    public static void ThrowIfInvalidCommissionLevel(int level)
    {
        if (level < 1)
            throw new InvalidCommissionLevelException(level);
    }
}