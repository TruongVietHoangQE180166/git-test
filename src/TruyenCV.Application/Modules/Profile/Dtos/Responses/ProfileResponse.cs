using System;
using TruyenCV.Domain.Enums;

namespace TruyenCV.Application.Modules.Profile.DTOs.Responses;

public class ProfileResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public string? PhoneNumber { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public string Slug { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public string Username { get; set; } = string.Empty;
}
