namespace Valetax.Wallets.Api.Domain;

public sealed class Wallet
{
    public Guid OwnerId { get; private set; }

    public ICollection<WalletPayout> Payouts { get; private set; } = [];

    public static Wallet Create(Guid ownerId)
    {
        var wallet = new Wallet
        {
            OwnerId = ownerId,
        };

        return wallet;
    }
}