using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Valetax.Api.Persistence;

public sealed class CommissionsDbContextFactory : IDesignTimeDbContextFactory<CommissionsDbContext>
{
    public CommissionsDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__CommissionsDbContext")
                               ?? "Host=localhost;Database=commissions;Username=postgres";
        var options = new DbContextOptionsBuilder<CommissionsDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new CommissionsDbContext(options);
    }
}