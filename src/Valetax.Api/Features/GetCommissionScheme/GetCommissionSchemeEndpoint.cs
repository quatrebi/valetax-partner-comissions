using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Valetax.Api.Persistence;
using Valetax.Infrastructure.Endpoints;

namespace Valetax.Api.Features.GetCommissionScheme;

public sealed class GetCommissionSchemeEndpoint : IApiEndpoint
{
    public void UseMetadata(IEndpointRouteBuilder routeBuilder) => routeBuilder
        .MapGroup(Constants.Api.AdminGroup)
        .MapGet("/commission-scheme", Handler);

    private static async Task<IResult> Handler(
        [FromServices] ICommissionsDbContext dbContext,
        CancellationToken ct = default)
    {
        var settings = await dbContext.CommissionSettings.AsNoTracking().SingleAsync(ct);

        return Results.Ok(GetCommissionSchemeDto.From(settings));
    }
}