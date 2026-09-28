using System.Text.Json;

namespace Valetax.Infrastructure.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; private set; }
    public string Topic { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset NextAttemptAt { get; private set; }

    public static OutboxMessage Create<TMessage>(string topic, TMessage message)
    {
        var now = DateTimeOffset.UtcNow;

        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Topic = topic,
            Payload = JsonSerializer.Serialize(message),
            CreatedAt = now,
            NextAttemptAt = now
        };
    }

    public TMessage GetPayload<TMessage>() => JsonSerializer.Deserialize<TMessage>(Payload)!;
}