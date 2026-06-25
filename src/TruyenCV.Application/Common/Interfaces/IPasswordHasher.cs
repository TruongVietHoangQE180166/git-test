namespace TruyenCV.Application.Common.Interfaces;

/// <summary>
/// Contract for hashing and verifying passwords.
/// Abstracts the underlying hashing algorithm (BCrypt) from the application layer.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>Hashes a plain-text password.</summary>
    string Hash(string plainTextPassword);

    /// <summary>
    /// Verifies a plain-text password against a stored hash.
    /// Returns <c>true</c> if they match.
    /// </summary>
    bool Verify(string plainTextPassword, string hashedPassword);
}
