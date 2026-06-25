using System;
using TruyenCV.Domain.Enums;

namespace TruyenCV.Application.Modules.Profile.DTOs.Requests;

public class UpdateProfileRequest
{
    public string FullName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public string? PhoneNumber { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public bool IsPublic { get; set; }
}
