using Microsoft.EntityFrameworkCore;
using Valetax.Infrastructure.Outbox;
using Valetax.Infrastructure.Persistence;
using Valetax.Partners.Api.Domain;

namespace Valetax.Partners.Api.Persistence;

public sealed class PartnersDbContext(
    DbContextOptions<PartnersDbContext> options
) :
    ValetaxBaseDbContext<PartnersDbContext>(options),
    IPartnersDbContext
{
    public DbSet<Partner> Partners => Set<Partner>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
}