using Microsoft.EntityFrameworkCore;
using TruyenCV.Domain.Entities;
using TruyenCV.Infrastructure.Persistence.Context;

using TruyenCV.Application.Modules.Profile.Interfaces.Repositories;
using TruyenCV.Infrastructure.Persistence.Repositories;

namespace TruyenCV.Infrastructure.Modules.Profile.Repositories;

/// <summary>
/// Repository for profile-specific queries and commands.
/// </summary>
public class ProfileRepository : Repository<Domain.Entities.Profile>, IProfileRepository
{
    public ProfileRepository(ApplicationDbContext context) : base(context)
    {
    }

    // ── Queries ──────────────────────────────────────────────────────────────

    /// <summary>Returns the profile for a given user, optionally including the User.</summary>
    public async Task<Domain.Entities.Profile?> GetByUserIdAsync(
        Guid userId,
        bool includeUser = false,
        CancellationToken ct = default)
    {
        var query = Context.Profiles.AsQueryable();

        if (includeUser)
            query = query.Include(p => p.User);

        return await query.FirstOrDefaultAsync(p => p.UserId == userId 
            && p.Status != Domain.Enums.EntityStatus.Deleted 
            && (!includeUser || p.User.Status != Domain.Enums.EntityStatus.Deleted), ct);
    }

    /// <summary>Returns a profile by URL slug (used for public profile pages).</summary>
    public async Task<Domain.Entities.Profile?> GetBySlugAsync(
        string slug,
        CancellationToken ct = default)
        => await Context.Profiles
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Slug == slug 
                && p.IsPublic 
                && p.Status != Domain.Enums.EntityStatus.Deleted 
                && p.User.Status != Domain.Enums.EntityStatus.Deleted, ct);

    public async Task<(IReadOnlyList<Domain.Entities.Profile> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? sortField,
        string? sortDirection,
        string? searchTerm,
        CancellationToken ct = default)
    {
        var query = Context.Profiles
            .Include(p => p.User)
            .Where(p => p.IsPublic 
                && p.Status != Domain.Enums.EntityStatus.Deleted 
                && p.User.Status != Domain.Enums.EntityStatus.Deleted)
            .AsQueryable();

        // Searching
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.FullName.Contains(searchTerm) || (p.Bio != null && p.Bio.Contains(searchTerm)));
        }

        // Sorting
        var isDescending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        query = sortField?.ToLowerInvariant() switch
        {
            "fullname" => isDescending ? query.OrderByDescending(p => p.FullName) : query.OrderBy(p => p.FullName),
            "updatedat" => isDescending ? query.OrderByDescending(p => p.UpdatedAt) : query.OrderBy(p => p.UpdatedAt),
            "createdat" => isDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.UpdatedAt)
        };

        // Paging
        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    // ── Existence Checks ─────────────────────────────────────────────────────

    public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct = default)
        => await Context.Profiles.AnyAsync(p => p.Slug == slug, ct);

    public async Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await Context.Profiles.AnyAsync(p => p.UserId == userId, ct);
}
