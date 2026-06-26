using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TruyenCV.Application.Modules.Auth.DTOs.Requests;
using TruyenCV.Application.Modules.Auth.DTOs.Responses;
using TruyenCV.Application.Modules.Auth.Interfaces.Services;
using TruyenCV.Shared.Responses;
using TruyenCV.Shared.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TruyenCV.API.Modules.Auth.Controllers;

[ApiController]
[Route("api/auth")]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private const string RefreshTokenCookieKey = "refreshToken";

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<object>>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var userId = await _authService.RegisterAsync(request, ct);
        return Ok(ApiResponse<object>.Success(new { userId }, "Registration successful. Please login to continue."));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var response = await _authService.LoginAsync(request, ct);
        SetRefreshTokenCookie(response.RefreshToken);
        return Ok(ApiResponse<AuthResponse>.Success(response, "Login successful."));
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Refresh(CancellationToken ct)
    {
        var refreshToken = Request.Cookies[RefreshTokenCookieKey];
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new UnauthorizedException("Refresh token is missing.");
        }

        var response = await _authService.RefreshTokenAsync(new RefreshTokenRequest { RefreshToken = refreshToken }, ct);
        SetRefreshTokenCookie(response.RefreshToken);
        return Ok(ApiResponse<AuthResponse>.Success(response, "Token refreshed successfully."));
    }

    [HttpPost("revoke")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> Revoke(CancellationToken ct)
    {
        var refreshToken = Request.Cookies[RefreshTokenCookieKey];
        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            await _authService.RevokeSessionAsync(refreshToken, ct);
        }

        DeleteRefreshTokenCookie();
        return Ok(ApiResponse<object>.Success(new { }, "Session revoked successfully."));
    }

    private void SetRefreshTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps, // Secure if requested over HTTPS
            SameSite = SameSiteMode.Lax, // Safeguard against CSRF
            Expires = DateTimeOffset.UtcNow.AddDays(7) // Matches DB lifetime
        };

        Response.Cookies.Append(RefreshTokenCookieKey, token, cookieOptions);
    }

    private void DeleteRefreshTokenCookie()
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax
        };

        Response.Cookies.Delete(RefreshTokenCookieKey, cookieOptions);
    }
}
