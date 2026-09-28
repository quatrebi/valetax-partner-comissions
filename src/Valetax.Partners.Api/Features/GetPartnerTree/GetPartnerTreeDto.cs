namespace Valetax.Partners.Api.Features.GetPartnerTree;

public sealed record GetPartnerTreeDto(
    PartnerTreeNodeDto Partner,
    IReadOnlyList<PartnerTreeNodeDto> Ancestors,
    IReadOnlyList<PartnerTreeNodeDto> Referrals);

public sealed record PartnerTreeNodeDto(
    Guid ExternalId,
    Guid? RefPartnerExternalId,
    int Level);