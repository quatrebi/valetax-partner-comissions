using Grpc.Core;

namespace Valetax.Wallets.Api.Exceptions;

public sealed class InvalidOwnerIdException(string ownerId)
    : RpcException(new Status(StatusCode.InvalidArgument, $"Owner ID '{ownerId}' is invalid."));