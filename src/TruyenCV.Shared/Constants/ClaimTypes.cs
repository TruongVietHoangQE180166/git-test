namespace TruyenCV.Shared.Constants;

/// <summary>
/// JWT claim type names used when building and reading tokens.
/// </summary>
public static class AppClaimTypes
{
    /// <summary>The unique identifier of the authenticated user.</summary>
    public const string UserId = "uid";

    /// <summary>The email address of the authenticated user.</summary>
    public const string Email = "email";

    /// <summary>The role(s) assigned to the authenticated user.</summary>
    public const string Role = "role";

    /// <summary>The active auth session / refresh token identifier.</summary>
    public const string SessionId = "sid";

    /// <summary>The username / display name of the authenticated user.</summary>
    public const string Username = "username";
}
