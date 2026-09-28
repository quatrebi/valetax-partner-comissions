using Microsoft.EntityFrameworkCore;
using Valetax.Infrastructure.Outbox;

namespace Valetax.Infrastructure.Persistence;

public abstract class ValetaxBaseDbContext<TDbContext>(DbContextOptions<TDbContext> options) : DbContext(options), IDbContext
    where TDbContext : DbContext, IDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        if (this is IOutboxDbContext)
            modelBuilder.ApplyConfiguration(new OutboxMessageEntityTypeConfiguration());
    }
}