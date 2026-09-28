using Microsoft.EntityFrameworkCore;
using Valetax.Infrastructure.Persistence;
using Valetax.Wallets.Api.Domain;

namespace Valetax.Wallets.Api.Persistence;

public interface IWalletsDbContext : IDbContext
{
    DbSet<Wallet> Wallets { get; }
    DbSet<WalletPayout> Payouts { get; }
}