using Grpc.Core;

namespace Valetax.Wallets.Api.Exceptions;

public sealed class WalletNotFoundException(Guid ownerId)
    : RpcException(new Status(StatusCode.NotFound, $"Wallet '{ownerId}' not found."));