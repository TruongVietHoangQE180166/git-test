using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TruyenCV.Application.Common.Interfaces;
using TruyenCV.Shared.Exceptions;

namespace TruyenCV.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private async Task SendEmailAsync(string toEmail, string toName, string subject, string body, CancellationToken ct)
    {
        var host = _configuration["Email:Host"];
        var portStr = _configuration["Email:Port"];
        var username = _configuration["Email:Username"];
        var password = _configuration["Email:Password"];
        var fromEmail = _configuration["Email:FromEmail"];
        var fromName = _configuration["Email:FromName"];

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fromEmail))
        {
            _logger.LogWarning("Email settings are not fully configured. Skipping email send to {ToEmail}.", toEmail);
            return;
        }

        if (!int.TryParse(portStr, out var port))
        {
            port = 587; // Default SMTP port
        }

        try
        {
            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(new MailAddress(toEmail, toName));

            await client.SendMailAsync(mailMessage, ct);
            _logger.LogInformation("Email sent successfully to {ToEmail} with subject {Subject}", toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {ToEmail} with subject {Subject}", toEmail, subject);
            throw new ExternalServiceException("EmailService", "Failed to send email.", ex);
        }
    }

    public async Task SendVerificationEmailAsync(string toEmail, string toName, string verificationLink, CancellationToken ct = default)
    {
        var subject = "Verify your email address";
        var body = $"<p>Hi {toName},</p><p>Please verify your email address by clicking the link below:</p><p><a href=\"{verificationLink}\">Verify Email</a></p>";
        await SendEmailAsync(toEmail, toName, subject, body, ct);
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string toName, string resetLink, CancellationToken ct = default)
    {
        var subject = "Reset your password";
        var body = $"<p>Hi {toName},</p><p>You requested a password reset. Click the link below to set a new password:</p><p><a href=\"{resetLink}\">Reset Password</a></p>";
        await SendEmailAsync(toEmail, toName, subject, body, ct);
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string toName, CancellationToken ct = default)
    {
        var subject = "Welcome to TruyenCV!";
        var body = $"<p>Hi {toName},</p><p>Welcome to TruyenCV! We are glad to have you on board.</p>";
        await SendEmailAsync(toEmail, toName, subject, body, ct);
    }

    public async Task SendNotificationEmailAsync(string toEmail, string toName, string subject, string body, CancellationToken ct = default)
    {
        await SendEmailAsync(toEmail, toName, subject, body, ct);
    }
}
