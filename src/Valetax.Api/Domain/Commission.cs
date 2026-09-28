namespace Valetax.Api.Domain;

public sealed class Commission
{
    public Guid Id { get; private set; }
    public Guid ProfitEventId { get; private set; }
    public Guid RecipientExternalId { get; private set; }
    public int Level { get; private set; }
    public decimal Amount { get; private set; }
    public CommissionSchemeType SchemaType { get; private set; }
    public CommissionPaymentStatus PaymentStatus { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? PaidAt { get; private set; }

    public static Commission Create(
        Guid profitEventId,
        Guid recipientExternalId,
        int level,
        decimal amount,
        CommissionSchemeType schemaType) => new()
        {
            Id = Guid.NewGuid(),
            ProfitEventId = profitEventId,
            RecipientExternalId = recipientExternalId,
            Level = level,
            Amount = amount,
            SchemaType = schemaType,
            PaymentStatus = CommissionPaymentStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow
        };

    public void MarkPaid()
    {
        PaymentStatus = CommissionPaymentStatus.Paid;
        PaidAt = DateTimeOffset.UtcNow;
    }
}