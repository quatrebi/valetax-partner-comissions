using Grpc.Core;
using Valetax.Api.Exceptions;
using Valetax.Partners.Api.Grpc;

namespace Valetax.Api.Infrastructure;

public sealed class PartnersClient(PartnersService.PartnersServiceClient partnersClient) : IPartnersClient
{
    private static readonly TimeSpan CallTimeout = TimeSpan.FromSeconds(5);

    public async Task<PartnerChain?> GetPartnerChainAsync(Guid partnerExternalId, CancellationToken ct)
    {
        GetPartnerAncestorsResponse response;
        try
        {
            response = await partnersClient.GetPartnerAncestorsAsync(
                new GetPartnerAncestorsRequest { PartnerExternalId = partnerExternalId.ToString() },
                deadline: DateTime.UtcNow.Add(CallTimeout),
                cancellationToken: ct);
        }
        catch (RpcException exception) when (exception.StatusCode == StatusCode.NotFound)
        {
            return null;
        }

        if (response.Nodes.Count == 0 ||
            response.Nodes[0].Level != 0 ||
            response.Nodes[0].ExternalId != partnerExternalId.ToString())
            throw new InvalidPartnerChainException(partnerExternalId);

        var refPartnerIds = new List<Guid>();
        var seen = new HashSet<Guid> { partnerExternalId };

        foreach (var node in response.Nodes.Skip(1))
        {
            if (!Guid.TryParse(node.ExternalId, out var refPartnerId) ||
                !seen.Add(refPartnerId) ||
                node.Level != refPartnerIds.Count + 1)
                throw new InvalidPartnerChainException(partnerExternalId);

            refPartnerIds.Add(refPartnerId);
        }

        return new PartnerChain(refPartnerIds);
    }
}