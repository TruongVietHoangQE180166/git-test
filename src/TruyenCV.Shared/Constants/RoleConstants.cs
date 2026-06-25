namespace TruyenCV.Shared.Constants;

/// <summary>
/// Application role name constants.
/// </summary>
public static class RoleConstants
{
    public const string Admin = "Admin";
    public const string User = "User";
    public const string Moderator = "Moderator";

    /// <summary>All defined roles as an array, useful for seeding.</summary>
    public static readonly string[] All = [Admin, Moderator, User];
}
