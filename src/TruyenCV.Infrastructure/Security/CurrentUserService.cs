using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TruyenCV.Application.Common.Interfaces;
using TruyenCV.Shared.Constants;

namespace TruyenCV.Infrastructure.Security;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var userIdClaim = User?.FindFirst(AppClaimTypes.UserId)?.Value 
                              ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            return null;
        }
    }

    public string? Email => User?.FindFirst(AppClaimTypes.Email)?.Value 
                            ?? User?.FindFirst(ClaimTypes.Email)?.Value;

    public string? Role => User?.FindFirst(AppClaimTypes.Role)?.Value 
                           ?? User?.FindFirst(ClaimTypes.Role)?.Value;

    public string? SessionId => User?.FindFirst(AppClaimTypes.SessionId)?.Value;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
