using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valetax.Api.Domain;

namespace Valetax.Api.Persistence.EntityTypeConfigurations;

public sealed class CommissionEntityTypeConfiguration : IEntityTypeConfiguration<Commission>
{
    public void Configure(EntityTypeBuilder<Commission> builder)
    {
        builder.ToTable("Commissions");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ProfitEventId, x.Level }).IsUnique();

        builder.Property(x => x.Amount).HasPrecision(Constants.Money.Precision, Constants.Money.Scale);
        builder.Property(x => x.SchemaType);
        builder.Property(x => x.PaymentStatus);

        builder.HasOne<ProfitEvent>()
            .WithMany(x => x.Commissions)
            .HasForeignKey(x => x.ProfitEventId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}