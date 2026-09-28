using Microsoft.Extensions.Options;
using Valetax.Api.Domain;
using Valetax.Api.Services;
using Xunit;

namespace Valetax.Api.Tests.Services;

public sealed class CommissionServiceTests
{
    private readonly CommissionService _service;

    public CommissionServiceTests()
    {
        _service = new CommissionService(Options.Create(new CommissionRoundingOptions()));
    }

    public static TheoryData<int, decimal> LinearCases => new()
    {
        { 1, 10m },
        { 2, 20m },
        { 3, 30m },
        { 10, 100m }
    };

    public static TheoryData<int, decimal> FibonacciCases => new()
    {
        { 1, 10m },
        { 2, 10m },
        { 3, 20m },
        { 4, 30m },
        { 5, 50m },
        { 10, 550m }
    };

    public static TheoryData<decimal, int, CommissionSchemeType, decimal> RoundingCases => new()
    {
        { 150m, 1, CommissionSchemeType.Linear, 1.50m },
        { 1.99m, 1, CommissionSchemeType.Linear, 0.01m },
        { 123.45m, 3, CommissionSchemeType.Linear, 3.70m },
        { 99.99m, 2, CommissionSchemeType.Fibonacci, 0.99m },
        { 0.33m, 10, CommissionSchemeType.Fibonacci, 0.18m }
    };

    public static TheoryData<decimal> NonPositiveProfits => [0m, -0.01m, -1000m];

    [Theory(DisplayName = "Linear scheme pays level% of the profit")]
    [MemberData(nameof(LinearCases))]
    public void Calculate_LinearScheme_ReturnsLevelPercentOfProfit(int level, decimal expected)
    {
        var commission = _service.Calculate(1000m, level, CommissionSchemeType.Linear);

        Assert.Equal(expected, commission);
    }

    [Theory(DisplayName = "Fibonacci scheme pays Fibonacci(level)% of the profit")]
    [MemberData(nameof(FibonacciCases))]
    public void Calculate_FibonacciScheme_ReturnsFibonacciPercentOfProfit(int level, decimal expected)
    {
        var commission = _service.Calculate(1000m, level, CommissionSchemeType.Fibonacci);

        Assert.Equal(expected, commission);
    }

    [Theory(DisplayName = "Fractional commission is rounded down to 2 decimals")]
    [MemberData(nameof(RoundingCases))]
    public void Calculate_FractionalCommission_RoundsDownToTwoDecimals(
        decimal profit,
        int level,
        CommissionSchemeType schemaType,
        decimal expected)
    {
        var commission = _service.Calculate(profit, level, schemaType);

        Assert.Equal(expected, commission);
    }

    [Theory(DisplayName = "Zero or negative profit gives no commission")]
    [MemberData(nameof(NonPositiveProfits))]
    public void Calculate_NonPositiveProfit_ReturnsZero(decimal profit)
    {
        Assert.Equal(0m, _service.Calculate(profit, 1, CommissionSchemeType.Linear));
        Assert.Equal(0m, _service.Calculate(profit, 1, CommissionSchemeType.Fibonacci));
    }

    [Theory(DisplayName = "Fibonacci(n) returns the n-th Fibonacci number")]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(3, 2)]
    [InlineData(10, 55)]
    [InlineData(20, 6765)]
    public void Fibonacci_ReturnsNthFibonacciNumber(int n, int expected)
    {
        Assert.Equal(expected, (int)CommissionService.Fibonacci(n));
    }
}