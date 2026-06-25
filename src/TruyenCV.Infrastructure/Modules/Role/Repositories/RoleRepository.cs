using Microsoft.EntityFrameworkCore;
using TruyenCV.Domain.Entities;
using TruyenCV.Infrastructure.Persistence.Context;

using TruyenCV.Application.Modules.Role.Interfaces.Repositories;
using TruyenCV.Infrastructure.Persistence.Repositories;

namespace TruyenCV.Infrastructure.Modules.Role.Repositories;

/// <summary>
/// Repository for role queries and commands.
/// </summary>
public class RoleRepository : Repository<Domain.Entities.Role>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext context) : base(context)
    {
    }

    // ── Queries ──────────────────────────────────────────────────────────────

    public async Task<Domain.Entities.Role?> GetByNameAsync(
        string name,
        CancellationToken ct = default)
        => await Context.Roles
            .FirstOrDefaultAsync(r => r.Name == name, ct);

    public override async Task<IReadOnlyList<Domain.Entities.Role>> GetAllAsync(
        CancellationToken ct = default)
        => await Context.Roles
            .OrderBy(r => r.Name)
            .ToListAsync(ct);

    /// <summary>Returns all roles with the number of assigned users.</summary>
    public async Task<IReadOnlyList<(Domain.Entities.Role Role, int UserCount)>> GetAllWithUserCountAsync(
        CancellationToken ct = default)
    {
        var result = await Context.Roles
            .Select(r => new { Role = r, UserCount = r.Users.Count })
            .OrderBy(x => x.Role.Name)
            .ToListAsync(ct);

        return result.Select(x => (x.Role, x.UserCount)).ToList();
    }

    // ── Existence Checks ─────────────────────────────────────────────────────

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
        => await Context.Roles.AnyAsync(r => r.Name == name, ct);

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default)
        => await Context.Roles.AnyAsync(r => r.Id == id, ct);

    // ── Commands ─────────────────────────────────────────────────────────────

    public override async Task AddAsync(Domain.Entities.Role role, CancellationToken ct = default)
    {
        if (role.Name is Shared.Constants.RoleConstants.Admin or Shared.Constants.RoleConstants.User)
            throw new Shared.Exceptions.BusinessRuleException($"Role '{role.Name}' is a reserved system role and cannot be created manually.");

        await base.AddAsync(role, ct);
    }

    public override void Update(Domain.Entities.Role role)
    {
        if (role.Name is Shared.Constants.RoleConstants.Admin or Shared.Constants.RoleConstants.User)
            throw new Shared.Exceptions.BusinessRuleException($"Role '{role.Name}' is a reserved system role and cannot be modified.");

        base.Update(role);
    }

    public override void Delete(Domain.Entities.Role role)
    {
        if (role.Name is Shared.Constants.RoleConstants.Admin or Shared.Constants.RoleConstants.User)
            throw new Shared.Exceptions.BusinessRuleException($"Role '{role.Name}' is a reserved system role and cannot be deleted.");

        base.Delete(role);
    }

    public async Task<(IReadOnlyList<Domain.Entities.Role> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? sortField,
        string? sortDirection,
        string? searchTerm,
        CancellationToken ct = default)
    {
        var query = Context.Roles.AsQueryable();

        // Searching
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(r => r.Name.Contains(searchTerm));
        }

        // Sorting
        var isDescending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        query = sortField?.ToLowerInvariant() switch
        {
            "name" => isDescending ? query.OrderByDescending(r => r.Name) : query.OrderBy(r => r.Name),
            "createdat" => isDescending ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt),
            _ => query.OrderBy(r => r.Name)
        };

        // Paging
        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }
}
