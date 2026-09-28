using Microsoft.EntityFrameworkCore;
using Valetax.Api.Domain;
using Valetax.Infrastructure.Persistence;

namespace Valetax.Api.Persistence;

public sealed class CommissionsDbContext(
    DbContextOptions<CommissionsDbContext> options
) :
    ValetaxBaseDbContext<CommissionsDbContext>(options),
    ICommissionsDbContext
{
    public DbSet<ProfitEvent> ProfitEvents => Set<ProfitEvent>();
    public DbSet<CommissionSettings> CommissionSettings => Set<CommissionSettings>();
}