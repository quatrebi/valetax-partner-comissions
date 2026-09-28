using Valetax.Api.Domain;

namespace Valetax.Api.Features.GetCommissionScheme;

public sealed record GetCommissionSchemeDto(
    CommissionSchemeType SchemaType,
    DateTimeOffset? UpdatedAt)
{
    public static GetCommissionSchemeDto From(CommissionSettings settings) => new(
        settings.SchemaType,
        settings.UpdatedAt);
}