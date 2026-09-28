using Grpc.Core;

namespace Valetax.Wallets.Api.Exceptions;

public sealed class InvalidPayoutCommissionRequestException(string commissionId)
    : RpcException(new Status(
        StatusCode.InvalidArgument,
        $"Payout request for commission '{commissionId}' is invalid."));