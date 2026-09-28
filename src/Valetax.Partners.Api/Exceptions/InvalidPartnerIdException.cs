using Grpc.Core;

namespace Valetax.Partners.Api.Exceptions;

public sealed class InvalidPartnerIdException(string partnerExternalId)
    : RpcException(new Status(StatusCode.InvalidArgument, $"Partner ID '{partnerExternalId}' is invalid."));