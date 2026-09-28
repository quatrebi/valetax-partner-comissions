using MassTransit;
using Microsoft.EntityFrameworkCore;
using Valetax.Infrastructure.Persistence;
using Valetax.Partners.Api.Contracts.IntegrationEvents;
using Valetax.Wallets.Api.Domain;
using Valetax.Wallets.Api.Persistence;

namespace Valetax.Wallets.Api.Features.CreateWallet;

public sealed class PartnerCreatedConsumer(
    IWalletsDbContext dbContext,
    ILogger<PartnerCreatedConsumer> logger) : IConsumer<PartnerCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PartnerCreatedIntegrationEvent> ctx)
    {
        var ownerId = ctx.Message.PartnerExternalId;

        if (await dbContext.Wallets.AnyAsync(x => x.OwnerId == ownerId, ctx.CancellationToken))
            return;

        await dbContext.Wallets.AddAsync(Wallet.Create(ownerId), ctx.CancellationToken);

        try
        {
            await dbContext.SaveChangesAsync(ctx.CancellationToken);
            logger.LogInformation("Wallet created for partner {PartnerExternalId}", ownerId);
        }
        catch (DbUpdateException exception) when (exception.IsUniqueViolation)
        {
        }
    }
}