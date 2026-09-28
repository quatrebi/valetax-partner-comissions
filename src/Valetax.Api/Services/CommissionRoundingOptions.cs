namespace Valetax.Api.Services;

public sealed class CommissionRoundingOptions
{
    public const string SectionName = "CommissionRounding";

    public MidpointRounding Mode { get; set; } = MidpointRounding.ToNegativeInfinity;
}