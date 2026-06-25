using Microsoft.EntityFrameworkCore;
using TruyenCV.Domain.Entities;
using TruyenCV.Infrastructure.Persistence.Context;

using TruyenCV.Application.Modules.Auth.Interfaces.Repositories;
using TruyenCV.Infrastructure.Persistence.Repositories;

namespace TruyenCV.Infrastructure.Modules.Auth.Repositories;

/// <summary>
/// Repository for auth session (refresh token) operations.
/// </summary>
public class AuthRepository : Repository<AuthSession>, IAuthRepository
{
    public AuthRepository(ApplicationDbContext context) : base(context)
    {
    }

    // ── Session Queries ──────────────────────────────────────────────────────

    /// <summary>Returns the session matching the given refresh token, including its user.</summary>
    public async Task<AuthSession?> GetByRefreshTokenAsync(
        string refreshToken,
        CancellationToken ct = default)
        => await Context.AuthSessions
            .Include(s => s.User)
                .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken, ct);

    /// <summary>Returns the active (non-revoked, non-expired) session for a user, if any.</summary>
    public async Task<AuthSession?> GetActiveSessionByUserIdAsync(
        Guid userId,
        CancellationToken ct = default)
        => await Context.AuthSessions
            .Where(s => s.UserId == userId
                     && !s.IsRevoked
                     && s.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(ct);

    /// <summary>Returns all active sessions for a user.</summary>
    public async Task<IReadOnlyList<AuthSession>> GetActiveSessionsByUserIdAsync(
        Guid userId,
        CancellationToken ct = default)
        => await Context.AuthSessions
            .Where(s => s.UserId == userId && !s.IsRevoked && s.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);

    // ── Session Commands ─────────────────────────────────────────────────────

    /// <summary>Persists a new session to the database.</summary>
    public async Task CreateSessionAsync(AuthSession session, CancellationToken ct = default)
    {
        await base.AddAsync(session, ct);
    }

    /// <summary>Marks a single session as revoked.</summary>
    public async Task RevokeSessionAsync(string refreshToken, CancellationToken ct = default)
    {
        var session = await Context.AuthSessions
            .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken, ct);

        if (session is not null)
        {
            session.IsRevoked = true;
        }
    }

    /// <summary>Revokes ALL sessions for a user (e.g. on password change, account lock).</summary>
    public async Task RevokeAllUserSessionsAsync(Guid userId, CancellationToken ct = default)
    {
        await Context.AuthSessions
            .Where(s => s.UserId == userId && !s.IsRevoked)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsRevoked, true), ct);
    }

    /// <summary>Deletes expired sessions (maintenance / cleanup).</summary>
    public async Task DeleteExpiredSessionsAsync(CancellationToken ct = default)
    {
        await Context.AuthSessions
            .Where(s => s.ExpiresAt <= DateTime.UtcNow)
            .ExecuteDeleteAsync(ct);
    }
}
