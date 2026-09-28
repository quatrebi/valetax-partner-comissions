using System.Globalization;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Valetax.Infrastructure.Persistence;
using Valetax.Wallets.Api.Domain;
using Valetax.Wallets.Api.Exceptions;
using Valetax.Wallets.Api.Grpc;
using Valetax.Wallets.Api.Guards;
using Valetax.Wallets.Api.Persistence;

namespace Valetax.Wallets.Api.Features.PayoutCommission;

public sealed class PayoutCommissionGrpcService(IWalletsDbContext dbContext)
    : WalletPayoutsService.WalletPayoutsServiceBase
{
    public override async Task<PayoutCommissionResponse> PayoutCommission(
        PayoutCommissionRequest request,
        ServerCallContext ctx)
    {
        if (!Guid.TryParse(request.OwnerId, out var ownerId) ||
            !Guid.TryParse(request.CommissionId, out var commissionId) ||
            !decimal.TryParse(request.Amount, CultureInfo.InvariantCulture, out var amount) ||
            amount <= 0 ||
            amount != decimal.Round(amount, Constants.Money.Scale))
            throw new InvalidPayoutCommissionRequestException(request.CommissionId);

        var existing = await dbContext.Payouts.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CommissionId == commissionId, ctx.CancellationToken);

        if (existing is not null)
            return ExistingResponse(existing, ownerId, amount);

        if (!await dbContext.Wallets.AnyAsync(x => x.OwnerId == ownerId, ctx.CancellationToken))
            throw new WalletNotFoundException(ownerId);

        await dbContext.Payouts.AddAsync(new WalletPayout(
            commissionId,
            ownerId,
            amount,
            DateTimeOffset.UtcNow),
            ctx.CancellationToken);

        try
        {
            await dbContext.SaveChangesAsync(ctx.CancellationToken);
        }
        catch (DbUpdateException exception) when (exception.IsUniqueViolation)
        {
            existing = await dbContext.Payouts.AsNoTracking()
                .SingleAsync(x => x.CommissionId == commissionId, ctx.CancellationToken);

            return ExistingResponse(existing, ownerId, amount);
        }

        return new PayoutCommissionResponse { AlreadyPaid = false };
    }

    private static PayoutCommissionResponse ExistingResponse(WalletPayout existing, Guid ownerId, decimal amount)
    {
        ThrowIfPayoutAlreadyExistsGuard.ThrowIfPayoutAlreadyExists(existing, ownerId, amount);

        return new PayoutCommissionResponse { AlreadyPaid = true };
    }
}