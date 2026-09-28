using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Valetax.Wallets.Api.Persistence;

public sealed class WalletsDbContextFactory : IDesignTimeDbContextFactory<WalletsDbContext>
{
    public WalletsDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__WalletsDbContext")
                               ?? "Host=localhost;Database=wallets;Username=postgres";
        var options = new DbContextOptionsBuilder<WalletsDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new WalletsDbContext(options);
    }
}