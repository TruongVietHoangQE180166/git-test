using Microsoft.EntityFrameworkCore;
using TruyenCV.Domain.Entities;
using TruyenCV.Infrastructure.Persistence.Context;

using TruyenCV.Application.Modules.User.Interfaces.Repositories;
using TruyenCV.Infrastructure.Persistence.Repositories;

namespace TruyenCV.Infrastructure.Modules.User.Repositories;

/// <summary>
/// Repository for user-specific queries and commands.
/// </summary>
public class UserRepository : Repository<Domain.Entities.User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    // ── Queries ──────────────────────────────────────────────────────────────



    public async Task<Domain.Entities.User?> GetByEmailAsync(
        string email,
        CancellationToken ct = default)
        => await Context.Users
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);

    public async Task<Domain.Entities.User?> GetByUsernameAsync(
        string username,
        CancellationToken ct = default)
        => await Context.Users
            .FirstOrDefaultAsync(u => u.Username == username, ct);

    /// <summary>Returns the user including their Role and Profile.</summary>
    public async Task<Domain.Entities.User?> GetWithDetailsAsync(
        Guid id,
        CancellationToken ct = default)
        => await Context.Users
            .Include(u => u.Role)
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

    /// <summary>Returns the user including their Role (needed for JWT generation).</summary>
    public async Task<Domain.Entities.User?> GetWithRoleAsync(
        Guid id,
        CancellationToken ct = default)
        => await Context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

    public override async Task<IReadOnlyList<Domain.Entities.User>> GetAllAsync(
        CancellationToken ct = default)
        => await Context.Users
            .Include(u => u.Role)
            .OrderBy(u => u.CreatedAt)
            .ToListAsync(ct);

    // ── Existence Checks ─────────────────────────────────────────────────────

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        => await Context.Users
            .AnyAsync(u => u.Email == email.ToLowerInvariant(), ct);

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default)
        => await Context.Users
            .AnyAsync(u => u.Username == username, ct);

    // ── Commands ─────────────────────────────────────────────────────────────

    public override async Task AddAsync(Domain.Entities.User user, CancellationToken ct = default)
    {
        user.Email = user.Email.ToLowerInvariant();
        await base.AddAsync(user, ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        // 1. Soft delete User
        await Context.Users
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.Status, Domain.Enums.EntityStatus.Deleted), ct);

        // 2. Soft delete Profile
        await Context.Profiles
            .Where(p => p.UserId == id)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Status, Domain.Enums.EntityStatus.Deleted), ct);

        // 3. Invalidate and Soft delete active Sessions
        await Context.AuthSessions
            .Where(s => s.UserId == id)
            .ExecuteUpdateAsync(s => s.SetProperty(s => s.IsRevoked, true)
                                      .SetProperty(s => s.Status, Domain.Enums.EntityStatus.Deleted), ct);
    }

    public async Task BanAsync(Guid id, CancellationToken ct = default)
    {
        // 1. Set User status to Inactive
        await Context.Users
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.Status, Domain.Enums.EntityStatus.Inactive), ct);

        // 2. Set Profile status to Inactive
        await Context.Profiles
            .Where(p => p.UserId == id)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Status, Domain.Enums.EntityStatus.Inactive), ct);

        // 3. Revoke all active Sessions
        await Context.AuthSessions
            .Where(s => s.UserId == id)
            .ExecuteUpdateAsync(s => s.SetProperty(s => s.IsRevoked, true)
                                      .SetProperty(s => s.Status, Domain.Enums.EntityStatus.Inactive), ct);
    }

    public async Task UnbanAsync(Guid id, CancellationToken ct = default)
    {
        // 1. Set User status to Active
        await Context.Users
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.Status, Domain.Enums.EntityStatus.Active), ct);

        // 2. Set Profile status to Active
        await Context.Profiles
            .Where(p => p.UserId == id)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Status, Domain.Enums.EntityStatus.Active), ct);
    }

    public async Task<(IReadOnlyList<Domain.Entities.User> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? sortField,
        string? sortDirection,
        string? searchTerm,
        CancellationToken ct = default)
    {
        var query = Context.Users.Include(u => u.Role).AsQueryable();

        // 1. Searching
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u => u.Username.Contains(searchTerm) || u.Email.Contains(searchTerm));
        }

        // 2. Sorting
        var isDescending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        query = sortField?.ToLowerInvariant() switch
        {
            "username" => isDescending ? query.OrderByDescending(u => u.Username) : query.OrderBy(u => u.Username),
            "email" => isDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "createdat" => isDescending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
            _ => query.OrderBy(u => u.CreatedAt)
        };

        // 3. Paging
        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }
}
