namespace TruyenCV.Shared.Exceptions;

/// <summary>
/// Thrown when a business rule or domain invariant is violated (HTTP 422 Unprocessable Entity).
/// Examples: cannot delete an account with active subscriptions, CV already published.
/// </summary>
public sealed class BusinessRuleException : Exception
{
    /// <summary>A short machine-readable code identifying the rule that was violated.</summary>
    public string? RuleCode { get; }

    public BusinessRuleException(string message)
        : base(message) { }

    public BusinessRuleException(string message, string ruleCode)
        : base(message)
    {
        RuleCode = ruleCode;
    }

    public BusinessRuleException(string message, Exception innerException)
        : base(message, innerException) { }
}
