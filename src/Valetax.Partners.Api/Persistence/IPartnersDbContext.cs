using Microsoft.EntityFrameworkCore;
using Valetax.Infrastructure.Outbox;
using Valetax.Partners.Api.Domain;

namespace Valetax.Partners.Api.Persistence;

public interface IPartnersDbContext : IOutboxDbContext
{
    DbSet<Partner> Partners { get; }
}