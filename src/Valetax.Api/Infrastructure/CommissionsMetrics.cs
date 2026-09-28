using System.Diagnostics.Metrics;

namespace Valetax.Api.Infrastructure;

public sealed class CommissionsMetrics
{
    public const string MeterName = "Valetax.Api";

    private readonly Counter<long> _profitEventsAccepted;
    private readonly Counter<long> _commissionsAccrued;
    private readonly Counter<long> _commissionsPaid;
    private readonly Counter<long> _commissionPayoutFailures;
    private long _oldestPendingProfitEventAgeSeconds;

    public CommissionsMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);

        _profitEventsAccepted = meter.CreateCounter<long>(
            "valetax.profit_events.accepted",
            description: "Profit events accepted for processing.");
        _commissionsAccrued = meter.CreateCounter<long>(
            "valetax.commissions.accrued",
            description: "Commissions accrued for accepted profit events.");
        _commissionsPaid = meter.CreateCounter<long>(
            "valetax.commissions.paid",
            description: "Commissions paid out to wallets.");
        _commissionPayoutFailures = meter.CreateCounter<long>(
            "valetax.commissions.payout_failures",
            description: "Failed commission payout attempts.");
        meter.CreateObservableGauge(
            "valetax.profit_events.oldest_pending_age",
            () => Interlocked.Read(ref _oldestPendingProfitEventAgeSeconds),
            unit: "s",
            description: "Age of the oldest profit event with unpaid commissions.");
    }

    public void ProfitEventAccepted(int commissionsCount)
    {
        _profitEventsAccepted.Add(1);
        _commissionsAccrued.Add(commissionsCount);
    }

    public void CommissionPaid() => _commissionsPaid.Add(1);

    public void CommissionPayoutFailed(string reason) =>
        _commissionPayoutFailures.Add(1, new KeyValuePair<string, object?>("reason", reason));

    public void OldestPendingProfitEvent(DateTimeOffset? createdAt) =>
        Interlocked.Exchange(
            ref _oldestPendingProfitEventAgeSeconds,
            createdAt is { } value ? (long)(DateTimeOffset.UtcNow - value).TotalSeconds : 0);
}