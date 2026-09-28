using Grpc.Core;

namespace Valetax.Partners.Api.Exceptions;

public sealed class PartnerNotFoundException(Guid partnerExternalId)
    : RpcException(new Status(StatusCode.NotFound, $"Partner '{partnerExternalId}' not found."));