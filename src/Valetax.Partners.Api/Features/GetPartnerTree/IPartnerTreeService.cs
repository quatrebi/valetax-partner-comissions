namespace Valetax.Partners.Api.Features.GetPartnerTree;

public interface IPartnerTreeService
{
    Task<List<PartnerTreeNodeDto>> GetAncestorsAsync(Guid partnerId, CancellationToken ct);

    Task<GetPartnerTreeDto?> GetTreeAsync(Guid partnerId, CancellationToken ct);

    Task LockRefPartnerChangesAsync(CancellationToken ct);

    Task ThrowIfCannotSetRefPartnerAsync(Guid partnerId, Guid? refPartnerId, CancellationToken ct);
}