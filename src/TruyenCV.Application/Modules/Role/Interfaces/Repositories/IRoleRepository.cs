using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TruyenCV.Application.Common.Interfaces;

namespace TruyenCV.Application.Modules.Role.Interfaces.Repositories;

public interface IRoleRepository : IRepository<TruyenCV.Domain.Entities.Role>
{
    Task<TruyenCV.Domain.Entities.Role?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<IReadOnlyList<(TruyenCV.Domain.Entities.Role Role, int UserCount)>> GetAllWithUserCountAsync(CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<TruyenCV.Domain.Entities.Role> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? sortField,
        string? sortDirection,
        string? searchTerm,
        CancellationToken ct = default);
}
