using TruyenCV.Shared.Constants;

namespace TruyenCV.Shared.Helpers;

/// <summary>
/// Utility methods for pagination calculations.
/// </summary>
public static class PaginationHelper
{
    /// <summary>
    /// Calculates the total number of pages.
    /// </summary>
    public static int CalculateTotalPages(int totalCount, int pageSize)
    {
        if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
        if (totalCount <= 0) return 0;

        return (int)Math.Ceiling((double)totalCount / pageSize);
    }

    /// <summary>
    /// Calculates the number of items to skip (SQL OFFSET).
    /// </summary>
    public static int CalculateOffset(int page, int pageSize)
    {
        var validPage = NormalizePage(page);
        var validPageSize = NormalizePageSize(pageSize);
        return (validPage - 1) * validPageSize;
    }

    /// <summary>
    /// Returns <c>true</c> when a previous page exists.
    /// </summary>
    public static bool HasPreviousPage(int page) => page > 1;

    /// <summary>
    /// Returns <c>true</c> when a next page exists.
    /// </summary>
    public static bool HasNextPage(int page, int totalPages) => page < totalPages;

    /// <summary>
    /// Clamps the page number to a valid minimum (≥ 1).
    /// </summary>
    public static int NormalizePage(int page)
        => Math.Max(AppConstants.MinPageNumber, page);

    /// <summary>
    /// Clamps the page size within [1, MaxPageSize].
    /// </summary>
    public static int NormalizePageSize(int pageSize)
        => Math.Clamp(pageSize, 1, AppConstants.MaxPageSize);

    /// <summary>
    /// Returns the effective page size, defaulting to <see cref="AppConstants.DefaultPageSize"/> when ≤ 0.
    /// </summary>
    public static int ResolvePageSize(int? pageSize)
        => pageSize is null or <= 0
            ? AppConstants.DefaultPageSize
            : NormalizePageSize(pageSize.Value);
}
