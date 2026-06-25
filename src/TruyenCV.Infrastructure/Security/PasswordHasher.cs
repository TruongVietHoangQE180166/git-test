using TruyenCV.Application.Common.Interfaces;

namespace TruyenCV.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    // A work factor of 12 is a good balance between security and performance
    private const int WorkFactor = 12;

    public string Hash(string plainTextPassword)
    {
        if (string.IsNullOrWhiteSpace(plainTextPassword))
        {
            throw new ArgumentException("Password cannot be empty.", nameof(plainTextPassword));
        }

        return BCrypt.Net.BCrypt.EnhancedHashPassword(plainTextPassword, WorkFactor);
    }

    public bool Verify(string plainTextPassword, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(plainTextPassword))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(hashedPassword))
        {
            return false;
        }

        return BCrypt.Net.BCrypt.EnhancedVerify(plainTextPassword, hashedPassword);
    }
}
