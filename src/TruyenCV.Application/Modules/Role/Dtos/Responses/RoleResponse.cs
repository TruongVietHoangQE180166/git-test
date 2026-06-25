using System;

namespace TruyenCV.Application.Modules.Role.DTOs.Responses;

public class RoleResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int UserCount { get; set; }
}
