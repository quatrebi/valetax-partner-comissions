namespace Valetax.Api.Domain;

public sealed class ProfitEvent
{
    public Guid ExternalId { get; private set; }
    public Guid PartnerExternalId { get; private set; }
    public decimal Profit { get; private set; }
    public CommissionSchemeType SchemaType { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public ProfitEventStatus Status { get; private set; }
    public DateTimeOffset? NextAttemptAt { get; private set; }
    public ICollection<Commission> Commissions { get; private set; } = [];

    public static ProfitEvent Create(
        Guid externalId,
        Guid partnerExternalId,
        decimal profit,
        CommissionSchemeType schemaType)
    {
        var now = DateTimeOffset.UtcNow;

        return new ProfitEvent
        {
            ExternalId = externalId,
            PartnerExternalId = partnerExternalId,
            Profit = profit,
            SchemaType = schemaType,
            CreatedAt = now,
            Status = ProfitEventStatus.Pending,
            NextAttemptAt = now
        };
    }

    public void SetCommissions(IEnumerable<Commission> commissions)
    {
        foreach (var commission in commissions.Where(x => x.Amount > 0))
            Commissions.Add(commission);

        if (Commissions.Count == 0)
            Complete();
    }

    public void Complete()
    {
        Status = ProfitEventStatus.Completed;
        NextAttemptAt = null;
    }

    public void RetryAt(DateTimeOffset nextAttemptAt)
    {
        NextAttemptAt = nextAttemptAt;
    }
}