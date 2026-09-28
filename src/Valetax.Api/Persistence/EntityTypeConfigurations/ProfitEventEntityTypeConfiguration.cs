using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valetax.Api.Domain;

namespace Valetax.Api.Persistence.EntityTypeConfigurations;

public sealed class ProfitEventEntityTypeConfiguration : IEntityTypeConfiguration<ProfitEvent>
{
    private static readonly string PendingFilter = $"\"Status\" = {(int)ProfitEventStatus.Pending}";

    public void Configure(EntityTypeBuilder<ProfitEvent> builder)
    {
        builder.ToTable("ProfitEvents");
        builder.HasKey(x => x.ExternalId);

        builder.HasIndex(x => new { x.PartnerExternalId, x.CreatedAt });
        builder.HasIndex(x => x.NextAttemptAt).HasFilter(PendingFilter);
        builder.HasIndex(x => x.CreatedAt).HasFilter(PendingFilter);

        builder.Property(x => x.Profit).HasPrecision(Constants.Money.Precision, Constants.Money.Scale);
        builder.Property(x => x.SchemaType);
        builder.Property(x => x.Status);
    }
}