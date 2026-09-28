using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Valetax.Partners.Api.Persistence;

public sealed class PartnersDbContextFactory : IDesignTimeDbContextFactory<PartnersDbContext>
{
    public PartnersDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__PartnersDbContext")
                               ?? "Host=localhost;Database=partners;Username=postgres";
        var options = new DbContextOptionsBuilder<PartnersDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new PartnersDbContext(options);
    }
}