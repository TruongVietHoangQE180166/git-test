namespace TruyenCV.Application.Modules.User.DTOs.Requests;

public class UpdateUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
}
