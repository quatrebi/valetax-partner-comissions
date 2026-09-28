namespace Valetax.Api.Exceptions;

public sealed class InvalidPartnerChainException(Guid partnerExternalId)
    : InvalidOperationException($"Partners API returned an invalid ancestor chain for partner '{partnerExternalId}'.");