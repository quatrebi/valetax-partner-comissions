using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valetax.Wallets.Api.Domain;

namespace Valetax.Wallets.Api.Persistence.EntityTypeConfigurations;

public sealed class WalletEntityTypeConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("Wallets");
        builder.HasKey(x => x.OwnerId);

        builder.HasMany(x => x.Payouts)
            .WithOne()
            .HasForeignKey(x => x.WalletId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}