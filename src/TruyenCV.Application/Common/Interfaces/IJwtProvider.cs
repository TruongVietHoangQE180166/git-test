using System.Security.Claims;
using TruyenCV.Domain.Entities;

namespace TruyenCV.Application.Common.Interfaces;

/// <summary>
/// Contract for generating and validating JWT tokens.
/// </summary>
public interface IJwtProvider
{
    /// <summary>
    /// Generates a short-lived JWT access token for the given user.
    /// </summary>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Generates a cryptographically random refresh token (Base64 encoded).
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Extracts the <see cref="ClaimsPrincipal"/> from an expired access token
    /// (used during refresh-token rotation without re-validating lifetime).
    /// </summary>
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
