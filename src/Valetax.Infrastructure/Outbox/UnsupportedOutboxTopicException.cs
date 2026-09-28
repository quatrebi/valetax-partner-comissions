namespace Valetax.Infrastructure.Outbox;

public sealed class UnsupportedOutboxTopicException(string topic)
    : InvalidOperationException($"Outbox topic '{topic}' is not supported.");