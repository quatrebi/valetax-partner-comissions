using Microsoft.AspNetCore.Mvc;
using Valetax.Infrastructure.Endpoints;

namespace Valetax.Partners.Api.Features.GetPartnerTree;

public sealed class GetPartnerTreeEndpoint : IApiEndpoint
{
    public void UseMetadata(IEndpointRouteBuilder routeBuilder) => routeBuilder
        .MapGroup(Constants.Api.PartnersGroup)
        .MapGet("/{id:guid}/tree", Handler);

    private static async Task<IResult> Handler(
        [FromRoute] Guid id,
        [FromServices] IPartnerTreeService treeService,
        CancellationToken ct = default)
    {
        var tree = await treeService.GetTreeAsync(id, ct);

        return tree is null ? Results.NotFound() : Results.Ok(tree);
    }
}