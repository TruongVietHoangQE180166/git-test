using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TruyenCV.Shared.Helpers;

/// <summary>
/// Utility methods for string manipulation.
/// </summary>
public static partial class StringHelper
{
    // ── Slug ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Converts a string to a URL-friendly slug.
    /// Example: "Xin Chào Thế Giới!" → "xin-chao-the-gioi"
    /// </summary>
    public static string ToSlug(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        // Normalize unicode (decompose accented chars)
        var normalized = input.Normalize(NormalizationForm.FormD);

        // Remove non-ASCII characters (diacritics)
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        var slug = sb.ToString()
            .Normalize(NormalizationForm.FormC)
            .ToLowerInvariant();

        // Replace spaces and non-alphanumeric chars with hyphens
        slug = NonAlphanumericRegex().Replace(slug, "-");

        // Collapse multiple hyphens
        slug = MultipleHyphensRegex().Replace(slug, "-");

        return slug.Trim('-');
    }

    // ── Truncate ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Truncates a string to the specified maximum length, appending a suffix if truncated.
    /// </summary>
    public static string Truncate(string? input, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        if (input.Length <= maxLength) return input;

        return string.Concat(input.AsSpan(0, maxLength - suffix.Length), suffix);
    }

    // ── Null / Empty ─────────────────────────────────────────────────────────

    public static bool IsNullOrEmpty(string? value) => string.IsNullOrEmpty(value);

    public static bool IsNullOrWhiteSpace(string? value) => string.IsNullOrWhiteSpace(value);

    // ── Casing ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Capitalizes the first letter of a string.
    /// </summary>
    public static string Capitalize(string? input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        if (input.Length == 1) return input.ToUpperInvariant();
        return char.ToUpperInvariant(input[0]) + input[1..];
    }

    /// <summary>
    /// Converts a string to camelCase.
    /// </summary>
    public static string ToCamelCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        return char.ToLowerInvariant(input[0]) + input[1..];
    }

    // ── Masking ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Masks an email address for display: "user@example.com" → "us**@example.com".
    /// </summary>
    public static string MaskEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return string.Empty;

        var atIndex = email.IndexOf('@');
        if (atIndex <= 2) return email;

        var local = email[..atIndex];
        var domain = email[atIndex..];
        var masked = local[..2] + new string('*', local.Length - 2);

        return masked + domain;
    }

    // ── Regex helpers ────────────────────────────────────────────────────────

    [GeneratedRegex(@"[^a-z0-9\s-]")]
    private static partial Regex NonAlphanumericRegex();

    [GeneratedRegex(@"[\s-]+")]
    private static partial Regex MultipleHyphensRegex();
}
