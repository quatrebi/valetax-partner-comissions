namespace Valetax.Wallets.Api.Domain;

public sealed record WalletPayout(
    Guid CommissionId,
    Guid WalletId,
    decimal Amount,
    DateTimeOffset PaidAt);