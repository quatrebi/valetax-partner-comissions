using Microsoft.EntityFrameworkCore;
using Valetax.Infrastructure.Persistence;

namespace Valetax.Infrastructure.Outbox;

public interface IOutboxDbContext : IDbContext
{
    DbSet<OutboxMessage> OutboxMessages { get; }
}