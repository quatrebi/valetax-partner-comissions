namespace Valetax.Api.Jobs;

public sealed class CommissionPayoutOptions
{
    public const string SectionName = "CommissionPayout";

    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(5);
    public int BatchSize { get; set; } = 20;
    public TimeSpan LeaseDuration { get; set; } = TimeSpan.FromMinutes(2);
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(30);
}