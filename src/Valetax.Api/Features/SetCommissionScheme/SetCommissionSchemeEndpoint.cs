using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Valetax.Api.Features.GetCommissionScheme;
using Valetax.Api.Persistence;
using Valetax.Infrastructure.Endpoints;

namespace Valetax.Api.Features.SetCommissionScheme;

public sealed class SetCommissionSchemeEndpoint : IApiEndpoint<SetCommissionSchemeDto>
{
    public void UseMetadata(IEndpointRouteBuilder routeBuilder) => routeBuilder
        .MapGroup(Constants.Api.AdminGroup)
        .MapPut("/commission-scheme", Handler);

    private static async Task<IResult> Handler(
        [FromBody] SetCommissionSchemeDto dto,
        [FromServices] ICommissionsDbContext dbContext,
        CancellationToken ct = default)
    {
        var settings = await dbContext.CommissionSettings.SingleAsync(ct);

        settings.SetSchemaType(dto.SchemaType);

        await dbContext.SaveChangesAsync(ct);

        return Results.Ok(GetCommissionSchemeDto.From(settings));
    }
}