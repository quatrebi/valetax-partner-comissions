namespace Valetax.Partners.Api.Features.GetPartnerTree;

public sealed class PartnerTreeOptions
{
    public const string SectionName = "PartnerTree";

    public int MaxDepth { get; set; } = 10;
}