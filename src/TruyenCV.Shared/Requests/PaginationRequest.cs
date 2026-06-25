using System.ComponentModel;

namespace TruyenCV.Shared.Requests;

public class PaginationRequest
{
    [DefaultValue(1)]
    public int? PageNumber { get; set; } = 1;

    [DefaultValue(10)]
    public int? PageSize { get; set; } = 10;

    [DefaultValue("createdAt")]
    public string? SortField { get; set; } = "createdAt";

    [DefaultValue("asc")]
    public string? SortDirection { get; set; } = "asc";

    public string? SearchTerm { get; set; }
}
