using System.Numerics;
using Microsoft.Extensions.Options;
using Valetax.Api.Domain;
using Valetax.Api.Exceptions;
using Valetax.Api.Guards;

namespace Valetax.Api.Services;

public sealed class CommissionService(IOptions<CommissionRoundingOptions> roundingOptions) : ICommissionService
{
    public decimal Calculate(decimal profit, int level, CommissionSchemeType schemaType)
    {
        ThrowIfInvalidCommissionLevelGuard.ThrowIfInvalidCommissionLevel(level);

        if (profit <= 0)
            return 0;

        var multiplier = schemaType switch
        {
            CommissionSchemeType.Linear => level,
            CommissionSchemeType.Fibonacci => (decimal)Fibonacci(level),
            _ => throw new UnsupportedCommissionSchemeException(schemaType)
        };

        return Math.Round(multiplier * profit / 100, Constants.Money.Scale, roundingOptions.Value.Mode);
    }

    public static BigInteger Fibonacci(int n)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(n);

        var result = Matrix2X2.Identity;
        var q = new Matrix2X2(1, 1, 1, 0);

        while (n > 0)
        {
            if ((n & 1) == 1)
                result *= q;

            q *= q;
            n >>= 1;
        }

        return result.M12;
    }

    private readonly record struct Matrix2X2(
        BigInteger M11, BigInteger M12,
        BigInteger M21, BigInteger M22)
    {
        public static Matrix2X2 Identity => new(1, 0, 0, 1);

        public static Matrix2X2 operator *(Matrix2X2 a, Matrix2X2 b) => new(
            a.M11 * b.M11 + a.M12 * b.M21, a.M11 * b.M12 + a.M12 * b.M22,
            a.M21 * b.M11 + a.M22 * b.M21, a.M21 * b.M12 + a.M22 * b.M22);
    }
}