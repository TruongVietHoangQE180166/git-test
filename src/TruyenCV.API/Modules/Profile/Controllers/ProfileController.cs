using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TruyenCV.Application.Modules.Profile.DTOs.Requests;
using TruyenCV.Application.Modules.Profile.DTOs.Responses;
using TruyenCV.Application.Modules.Profile.Interfaces.Services;
using TruyenCV.Shared.Constants;
using TruyenCV.Shared.Exceptions;
using TruyenCV.Shared.Requests;
using TruyenCV.Shared.Responses;

namespace TruyenCV.API.Modules.Profile.Controllers;

[ApiController]
[Route("api/profiles")]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProfileResponse>>> GetByUserId(Guid userId, CancellationToken ct)
    {
        var profile = await _profileService.GetByUserIdAsync(userId, ct);
        
        if (!profile.IsPublic)
        {
            var isAuthenticated = User.Identity?.IsAuthenticated == true;
            if (!isAuthenticated)
            {
                throw new UnauthorizedException("This profile is private.");
            }

            var currentUserId = GetUserId();
            var isAdmin = User.IsInRole(RoleConstants.Admin);

            if (profile.UserId != currentUserId && !isAdmin)
            {
                throw new ForbiddenException("You do not have permission to view this private profile.");
            }
        }

        return Ok(ApiResponse<ProfileResponse>.Success(profile, "Profile retrieved successfully."));
    }

    [HttpGet("slug/{slug}")]
    [ProducesResponseType(typeof(ApiResponse<ProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProfileResponse>>> GetBySlug(string slug, CancellationToken ct)
    {
        var profile = await _profileService.GetBySlugAsync(slug, ct);
        return Ok(ApiResponse<ProfileResponse>.Success(profile, "Profile retrieved successfully."));
    }

    [HttpGet("public")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<ProfileResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<ProfileResponse>>>> GetPagedPublic(
        [FromQuery] PaginationRequest request,
        CancellationToken ct = default)
    {
        var result = await _profileService.GetPagedAsync(request, ct);
        return Ok(ApiResponse<PaginatedResponse<ProfileResponse>>.Success(result, "Public profiles retrieved successfully."));
    }

    [Authorize]
    [HttpPut("me")]
    [ProducesResponseType(typeof(ApiResponse<ProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProfileResponse>>> UpdateMyProfile(
        [FromBody] UpdateProfileRequest request,
        CancellationToken ct)
    {
        var userId = GetUserId();
        var response = await _profileService.UpdateAsync(userId, request, ct);
        return Ok(ApiResponse<ProfileResponse>.Success(response, "Profile updated successfully."));
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
