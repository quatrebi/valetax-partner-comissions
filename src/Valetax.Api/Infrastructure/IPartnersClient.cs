namespace Valetax.Api.Infrastructure;

public interface IPartnersClient
{
    Task<PartnerChain?> GetPartnerChainAsync(Guid partnerExternalId, CancellationToken ct);
}