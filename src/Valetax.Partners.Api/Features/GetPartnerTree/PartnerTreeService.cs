using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Valetax.Partners.Api.Guards;
using Valetax.Partners.Api.Persistence;

namespace Valetax.Partners.Api.Features.GetPartnerTree;

public sealed class PartnerTreeService(IPartnersDbContext dbContext, IOptions<PartnerTreeOptions> options) : IPartnerTreeService
{
    private const long RefPartnerLockKey = 0x5041_5254_4E45_5253;

    private int MaxDepth => options.Value.MaxDepth;

    public Task<List<PartnerTreeNodeDto>> GetAncestorsAsync(Guid partnerId, CancellationToken ct) =>
        GetAncestorsAsync(partnerId, MaxDepth, ct);

    public async Task<GetPartnerTreeDto?> GetTreeAsync(Guid partnerId, CancellationToken ct)
    {
        var ancestors = await GetAncestorsAsync(partnerId, ct);
        if (ancestors.Count == 0)
            return null;

        var referrals = await GetReferralsAsync(partnerId, ct);

        return new GetPartnerTreeDto(ancestors[0], [.. ancestors.Skip(1)], referrals);
    }

    // Serializes ref-partner changes so two concurrent requests cannot close a cycle (A -> B and B -> A).
    public async Task LockRefPartnerChangesAsync(CancellationToken ct)
    {
        await dbContext.Database.ExecuteSqlAsync($"SELECT pg_advisory_xact_lock({RefPartnerLockKey})", ct);
    }

    public async Task ThrowIfCannotSetRefPartnerAsync(Guid partnerId, Guid? refPartnerId, CancellationToken ct)
    {
        if (refPartnerId is not { } refId)
            return;

        var refAncestors = await GetAncestorsAsync(refId, int.MaxValue, ct);

        ThrowIfRefPartnerNotFoundGuard.ThrowIfRefPartnerNotFound(refId, refAncestors.Count > 0);
        ThrowIfPartnerReferralCycleGuard.ThrowIfPartnerReferralCycle(
            partnerId,
            refId,
            refAncestors.Select(x => x.ExternalId));
    }

    private Task<List<PartnerTreeNodeDto>> GetAncestorsAsync(Guid partnerId, int maxDepth, CancellationToken ct) => dbContext.Database
        .SqlQuery<PartnerTreeNodeDto>($"""
            WITH RECURSIVE ancestors AS (
                SELECT "ExternalId", "RefPartnerId", 0 AS "Level"
                FROM "Partners"
                WHERE "ExternalId" = {partnerId}
                UNION ALL
                SELECT parent."ExternalId", parent."RefPartnerId", child."Level" + 1
                FROM "Partners" AS parent
                INNER JOIN ancestors AS child
                    ON parent."ExternalId" = child."RefPartnerId"
                WHERE child."Level" < {maxDepth}
            ) CYCLE "ExternalId" SET "IsCycle" USING "Path"
            SELECT "ExternalId", "RefPartnerId" AS "RefPartnerExternalId", "Level"
            FROM ancestors
            WHERE NOT "IsCycle"
            ORDER BY "Level"
            """)
        .ToListAsync(ct);

    private Task<List<PartnerTreeNodeDto>> GetReferralsAsync(Guid partnerId, CancellationToken ct) => dbContext.Database
        .SqlQuery<PartnerTreeNodeDto>($"""
            WITH RECURSIVE referrals AS (
                SELECT "ExternalId", "RefPartnerId", 0 AS "Level"
                FROM "Partners"
                WHERE "ExternalId" = {partnerId}
                UNION ALL
                SELECT child."ExternalId", child."RefPartnerId", parent."Level" + 1
                FROM "Partners" AS child
                INNER JOIN referrals AS parent
                    ON child."RefPartnerId" = parent."ExternalId"
                WHERE parent."Level" < {MaxDepth}
            ) CYCLE "ExternalId" SET "IsCycle" USING "Path"
            SELECT "ExternalId", "RefPartnerId" AS "RefPartnerExternalId", "Level"
            FROM referrals
            WHERE NOT "IsCycle" AND "Level" > 0
            ORDER BY "Level", "ExternalId"
            """)
        .ToListAsync(ct);
}