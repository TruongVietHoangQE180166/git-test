using System;
using System.Threading;
using System.Threading.Tasks;
using TruyenCV.Application.Common.Interfaces;

namespace TruyenCV.Application.Modules.User.Interfaces.Repositories;

public interface IUserRepository : IRepository<TruyenCV.Domain.Entities.User>
{
    Task<TruyenCV.Domain.Entities.User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<TruyenCV.Domain.Entities.User?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<TruyenCV.Domain.Entities.User?> GetWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<TruyenCV.Domain.Entities.User?> GetWithRoleAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
    Task BanAsync(Guid id, CancellationToken ct = default);
    Task UnbanAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<TruyenCV.Domain.Entities.User> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? sortField,
        string? sortDirection,
        string? searchTerm,
        CancellationToken ct = default);
}
