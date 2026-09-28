using Grpc.Core;

namespace Valetax.Wallets.Api.Exceptions;

public sealed class PayoutAlreadyExistsException(Guid commissionId)
    : RpcException(new Status(
        StatusCode.FailedPrecondition,
        $"Payout of commission '{commissionId}' already exists with different data."));