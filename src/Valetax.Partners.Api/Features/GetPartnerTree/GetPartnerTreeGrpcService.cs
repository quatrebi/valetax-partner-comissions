using Grpc.Core;
using Valetax.Partners.Api.Exceptions;
using Valetax.Partners.Api.Grpc;

namespace Valetax.Partners.Api.Features.GetPartnerTree;

public sealed class GetPartnerTreeGrpcService(IPartnerTreeService treeService) : PartnersService.PartnersServiceBase
{
    public override async Task<GetPartnerAncestorsResponse> GetPartnerAncestors(
        GetPartnerAncestorsRequest request,
        ServerCallContext ctx)
    {
        if (!Guid.TryParse(request.PartnerExternalId, out var partnerId))
            throw new InvalidPartnerIdException(request.PartnerExternalId);

        var ancestors = await treeService.GetAncestorsAsync(partnerId, ctx.CancellationToken);

        if (ancestors.Count == 0)
            throw new PartnerNotFoundException(partnerId);

        var result = new GetPartnerAncestorsResponse();
        result.Nodes.AddRange(ancestors.Select(x => new PartnerAncestorNode
        {
            ExternalId = x.ExternalId.ToString(),
            Level = x.Level
        }));

        return result;
    }
}