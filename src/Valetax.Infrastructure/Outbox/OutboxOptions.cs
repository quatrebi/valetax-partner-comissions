namespace Valetax.Infrastructure.Outbox;

public sealed class OutboxOptions
{
    public const string SectionName = "Outbox";

    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(2);
    public int BatchSize { get; set; } = 100;
    public TimeSpan LeaseDuration { get; set; } = TimeSpan.FromMinutes(1);
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(10);
    public TimeSpan PublishTimeout { get; set; } = TimeSpan.FromSeconds(15);
}