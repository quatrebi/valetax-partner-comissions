namespace Valetax.Wallets.Api.Features.GetWallet;

public sealed record GetWalletDto(Guid OwnerId, decimal Balance);