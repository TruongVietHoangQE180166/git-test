using TruyenCV.Application.Common.Interfaces;
using TruyenCV.Domain.Entities;

namespace TruyenCV.Application.Modules.Auth.Interfaces.Repositories;

public interface IAuthRepository : IRepository<AuthSession>
{
    Task<AuthSession?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task<AuthSession?> GetActiveSessionByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<AuthSession>> GetActiveSessionsByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task CreateSessionAsync(AuthSession session, CancellationToken ct = default);
    Task RevokeSessionAsync(string refreshToken, CancellationToken ct = default);
    Task RevokeAllUserSessionsAsync(Guid userId, CancellationToken ct = default);
    Task DeleteExpiredSessionsAsync(CancellationToken ct = default);
}
