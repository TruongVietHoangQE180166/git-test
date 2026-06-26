using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TruyenCV.Application.Modules.User.DTOs.Requests;
using TruyenCV.Application.Modules.User.DTOs.Responses;
using TruyenCV.Application.Modules.User.Interfaces.Services;
using TruyenCV.Shared.Constants;
using TruyenCV.Shared.Exceptions;
using TruyenCV.Shared.Requests;
using TruyenCV.Shared.Responses;

namespace TruyenCV.API.Modules.User.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize(Policy = AuthPolicies.RequireAdminRole)]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<UserResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<UserResponse>>>> GetPaged([FromQuery] PaginationRequest request, CancellationToken ct)
    {
        var result = await _userService.GetPagedAsync(request, ct);
        return Ok(ApiResponse<PaginatedResponse<UserResponse>>.Success(result, "Users retrieved successfully."));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetById(Guid id, CancellationToken ct)
    {
        var user = await _userService.GetByIdAsync(id, ct);
        return Ok(ApiResponse<UserResponse>.Success(user, "User retrieved successfully."));
    }

    [HttpGet("username/{username}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetByUsername(string username, CancellationToken ct)
    {
        var user = await _userService.GetByUsernameAsync(username, ct);
        return Ok(ApiResponse<UserResponse>.Success(user, "User retrieved successfully."));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<UserResponse>>> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        if (id != GetUserId() && !User.IsInRole(RoleConstants.Admin))
        {
            throw new ForbiddenException("You do not have permission to update this user.");
        }

        var response = await _userService.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<UserResponse>.Success(response, "User updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        if (id != GetUserId() && !User.IsInRole(RoleConstants.Admin))
        {
            throw new ForbiddenException("You do not have permission to delete this user.");
        }

        await _userService.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Success(new { }, "User deleted successfully."));
    }

    [Authorize(Policy = AuthPolicies.RequireAdminRole)]
    [HttpPost("{id:guid}/ban")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Ban(Guid id, CancellationToken ct)
    {
        await _userService.BanAsync(id, ct);
        return Ok(ApiResponse<object>.Success(new { }, "User account has been suspended successfully."));
    }

    [Authorize(Policy = AuthPolicies.RequireAdminRole)]
    [HttpPost("{id:guid}/unban")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Unban(Guid id, CancellationToken ct)
    {
        await _userService.UnbanAsync(id, ct);
        return Ok(ApiResponse<object>.Success(new { }, "User account has been unsuspended successfully."));
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(AppClaimTypes.UserId)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedException("User ID claim not found or invalid.");
        }
        return userId;
    }
}
