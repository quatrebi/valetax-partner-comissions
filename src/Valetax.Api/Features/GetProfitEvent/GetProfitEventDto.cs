using Valetax.Api.Domain;

namespace Valetax.Api.Features.GetProfitEvent;

public sealed record GetProfitEventDto(
    Guid ExternalId,
    Guid PartnerExternalId,
    decimal Profit,
    CommissionSchemeType SchemaType,
    ProfitEventStatus Status,
    DateTimeOffset CreatedAt,
    IReadOnlyList<ProfitEventCommissionDto> Commissions)
{
    public static GetProfitEventDto From(ProfitEvent profitEvent) => new(
        profitEvent.ExternalId,
        profitEvent.PartnerExternalId,
        profitEvent.Profit,
        profitEvent.SchemaType,
        profitEvent.Status,
        profitEvent.CreatedAt,
        [.. profitEvent.Commissions.OrderBy(x => x.Level).Select(ProfitEventCommissionDto.From)]);
}

public sealed record ProfitEventCommissionDto(
    Guid Id,
    Guid RecipientExternalId,
    int Level,
    decimal Amount,
    CommissionSchemeType SchemaType,
    CommissionPaymentStatus PaymentStatus,
    DateTimeOffset? PaidAt)
{
    public static ProfitEventCommissionDto From(Commission commission) => new(
        commission.Id,
        commission.RecipientExternalId,
        commission.Level,
        commission.Amount,
        commission.SchemaType,
        commission.PaymentStatus,
        commission.PaidAt);
}