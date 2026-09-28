using Microsoft.EntityFrameworkCore;
using Valetax.Infrastructure.Persistence;
using Valetax.Wallets.Api.Domain;

namespace Valetax.Wallets.Api.Persistence;

public sealed class WalletsDbContext(DbContextOptions<WalletsDbContext> options) : ValetaxBaseDbContext<WalletsDbContext>(options), IWalletsDbContext
{
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<WalletPayout> Payouts => Set<WalletPayout>();
}