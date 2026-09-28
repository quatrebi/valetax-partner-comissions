using Valetax.Infrastructure.Exceptions;

namespace Valetax.Api.Exceptions;

public sealed class PartnerWalletNotProvisionedException(Guid partnerExternalId)
    : ValetaxException(
        $"Wallet of partner '{partnerExternalId}' is not provisioned yet.",
        StatusCodes.Status409Conflict,
        TimeSpan.FromSeconds(5));