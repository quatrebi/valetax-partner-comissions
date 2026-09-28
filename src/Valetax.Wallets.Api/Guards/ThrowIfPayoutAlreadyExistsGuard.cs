using Valetax.Wallets.Api.Domain;
using Valetax.Wallets.Api.Exceptions;

namespace Valetax.Wallets.Api.Guards;

public static class ThrowIfPayoutAlreadyExistsGuard
{
    public static void ThrowIfPayoutAlreadyExists(WalletPayout existing, Guid ownerId, decimal amount)
    {
        if (existing.WalletId != ownerId || existing.Amount != amount)
            throw new PayoutAlreadyExistsException(existing.CommissionId);
    }
}