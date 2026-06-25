namespace TruyenCV.Application.Common.Interfaces;

/// <summary>
/// Provides information about the currently authenticated user,
/// derived from the HTTP request's JWT claims.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>The authenticated user's ID. <c>null</c> if unauthenticated.</summary>
    Guid? UserId { get; }

    /// <summary>The authenticated user's email. <c>null</c> if unauthenticated.</summary>
    string? Email { get; }

    /// <summary>The authenticated user's role name. <c>null</c> if unauthenticated.</summary>
    string? Role { get; }

    /// <summary>The current session (refresh token) ID. <c>null</c> if unauthenticated.</summary>
    string? SessionId { get; }

    /// <summary>Whether the current request has a valid authenticated user.</summary>
    bool IsAuthenticated { get; }
}
