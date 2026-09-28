namespace Valetax.Wallets.Api.Features.GetWalletPayouts;

public sealed record GetWalletPayoutsDto(
    Guid OwnerId,
    decimal Balance,
    IReadOnlyList<WalletPayoutDto> Payouts);

public sealed record WalletPayoutDto(
    Guid CommissionId,
    decimal Amount,
    DateTimeOffset PaidAt);