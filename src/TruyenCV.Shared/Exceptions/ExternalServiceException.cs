namespace TruyenCV.Shared.Exceptions;

/// <summary>
/// Thrown when a call to an external service fails (HTTP 502 Bad Gateway).
/// Examples: email provider down, payment gateway timeout, third-party API error.
/// </summary>
public sealed class ExternalServiceException : Exception
{
    /// <summary>The name of the external service (e.g. "EmailService", "PaymentGateway").</summary>
    public string ServiceName { get; }

    public ExternalServiceException(string serviceName)
        : base($"The external service '{serviceName}' is currently unavailable.")
    {
        ServiceName = serviceName;
    }

    public ExternalServiceException(string serviceName, string message)
        : base(message)
    {
        ServiceName = serviceName;
    }

    public ExternalServiceException(string serviceName, string message, Exception innerException)
        : base(message, innerException)
    {
        ServiceName = serviceName;
    }
}
