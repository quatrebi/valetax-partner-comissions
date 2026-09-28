using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valetax.Partners.Api.Domain;

namespace Valetax.Partners.Api.Persistence.EntityTypeConfigurations;

public sealed class PartnerEntityTypeConfiguration : IEntityTypeConfiguration<Partner>
{
    public void Configure(EntityTypeBuilder<Partner> builder)
    {
        builder.ToTable("Partners");
        builder.HasKey(x => x.ExternalId);

        builder.HasOne<Partner>()
            .WithMany()
            .HasForeignKey(x => x.RefPartnerId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}