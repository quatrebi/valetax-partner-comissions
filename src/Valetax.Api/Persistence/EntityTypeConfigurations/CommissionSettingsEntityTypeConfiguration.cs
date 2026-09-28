using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valetax.Api.Domain;

namespace Valetax.Api.Persistence.EntityTypeConfigurations;

public sealed class CommissionSettingsEntityTypeConfiguration : IEntityTypeConfiguration<CommissionSettings>
{
    public void Configure(EntityTypeBuilder<CommissionSettings> builder)
    {
        builder.ToTable("CommissionSettings");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.SchemaType);

        builder.HasData(new
        {
            Id = 1,
            SchemaType = CommissionSchemeType.Linear,
            UpdatedAt = (DateTimeOffset?)null
        });
    }
}