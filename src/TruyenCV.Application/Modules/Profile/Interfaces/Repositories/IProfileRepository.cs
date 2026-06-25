using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TruyenCV.Application.Common.Interfaces;

namespace TruyenCV.Application.Modules.Profile.Interfaces.Repositories;

public interface IProfileRepository : IRepository<TruyenCV.Domain.Entities.Profile>
{
    Task<TruyenCV.Domain.Entities.Profile?> GetByUserIdAsync(Guid userId, bool includeUser = false, CancellationToken ct = default);
    Task<TruyenCV.Domain.Entities.Profile?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct = default);
    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<(IReadOnlyList<TruyenCV.Domain.Entities.Profile> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? sortField,
        string? sortDirection,
        string? searchTerm,
        CancellationToken ct = default);
}
