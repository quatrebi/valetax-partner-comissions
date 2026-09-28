using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valetax.Wallets.Api.Domain;

namespace Valetax.Wallets.Api.Persistence.EntityTypeConfigurations;

public sealed class WalletPayoutEntityTypeConfiguration : IEntityTypeConfiguration<WalletPayout>
{
    public void Configure(EntityTypeBuilder<WalletPayout> builder)
    {
        builder.ToTable("WalletPayouts", table =>
            table.HasCheckConstraint("CK_WalletPayouts_Amount", "\"Amount\" > 0"));
        builder.HasKey(x => x.CommissionId);

        builder.Property(x => x.Amount)
            .HasPrecision(Constants.Money.Precision, Constants.Money.Scale);

        builder.HasIndex(x => new { x.WalletId, x.PaidAt })
            .IncludeProperties(x => x.Amount);
    }
}