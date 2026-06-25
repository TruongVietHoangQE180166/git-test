namespace TruyenCV.Application.Common.Interfaces;

/// <summary>
/// Contract for sending transactional emails.
/// </summary>
public interface IEmailService
{
    /// <summary>Sends an account verification email with a confirmation link or OTP.</summary>
    Task SendVerificationEmailAsync(
        string toEmail,
        string toName,
        string verificationLink,
        CancellationToken ct = default);

    /// <summary>Sends a password reset email with a reset link.</summary>
    Task SendPasswordResetEmailAsync(
        string toEmail,
        string toName,
        string resetLink,
        CancellationToken ct = default);

    /// <summary>Sends a welcome email after successful registration.</summary>
    Task SendWelcomeEmailAsync(
        string toEmail,
        string toName,
        CancellationToken ct = default);

    /// <summary>Sends a generic notification email.</summary>
    Task SendNotificationEmailAsync(
        string toEmail,
        string toName,
        string subject,
        string body,
        CancellationToken ct = default);
}
