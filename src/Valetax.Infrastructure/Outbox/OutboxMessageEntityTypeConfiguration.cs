using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Valetax.Infrastructure.Outbox;

public sealed class OutboxMessageEntityTypeConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Topic).HasMaxLength(256);
        builder.Property(x => x.Payload).HasColumnType("jsonb");

        builder.HasIndex(x => x.NextAttemptAt);
    }
}