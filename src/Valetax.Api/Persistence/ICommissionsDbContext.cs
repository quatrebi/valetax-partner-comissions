using Microsoft.EntityFrameworkCore;
using Valetax.Api.Domain;
using Valetax.Infrastructure.Persistence;

namespace Valetax.Api.Persistence;

public interface ICommissionsDbContext : IDbContext
{
    DbSet<ProfitEvent> ProfitEvents { get; }
    DbSet<CommissionSettings> CommissionSettings { get; }
}