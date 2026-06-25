namespace TruyenCV.Shared.Responses;

/// <summary>
/// Paginated list response for list endpoints.
/// </summary>
/// <typeparam name="T">The type of each item in the result set.</typeparam>
public sealed class PaginationResponse<T>
{
    /// <summary>Items in the current page.</summary>
    public IReadOnlyList<T> Items { get; init; } = [];

    /// <summary>Current page number (1-based).</summary>
    public int Page { get; init; }

    /// <summary>Number of items per page.</summary>
    public int PageSize { get; init; }

    /// <summary>Total number of items across all pages.</summary>
    public int TotalCount { get; init; }

    /// <summary>Total number of pages.</summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    public bool HasPreviousPage => Page > 1;

    public bool HasNextPage => Page < TotalPages;

    // ── Factory Method ───────────────────────────────────────────────────────

    public static PaginationResponse<T> Create(
        IReadOnlyList<T> items,
        int page,
        int pageSize,
        int totalCount)
        => new()
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
}
