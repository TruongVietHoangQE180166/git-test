using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TruyenCV.Application.Modules.Role.DTOs.Requests;
using TruyenCV.Application.Modules.Role.DTOs.Responses;
using TruyenCV.Application.Modules.Role.Interfaces.Services;
using TruyenCV.Shared.Constants;
using TruyenCV.Shared.Requests;
using TruyenCV.Shared.Responses;

namespace TruyenCV.API.Modules.Role.Controllers;

[Authorize(Policy = AuthPolicies.RequireAdminRole)]
[ApiController]
[Route("api/roles")]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<RoleResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<RoleResponse>>>> GetPaged([FromQuery] PaginationRequest request, CancellationToken ct)
    {
        var result = await _roleService.GetPagedAsync(request, ct);
        return Ok(ApiResponse<PaginatedResponse<RoleResponse>>.Success(result, "Roles retrieved successfully."));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<RoleResponse>>> GetById(Guid id, CancellationToken ct)
    {
        var role = await _roleService.GetByIdAsync(id, ct);
        return Ok(ApiResponse<RoleResponse>.Success(role, "Role retrieved successfully."));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<RoleResponse>>> Create([FromBody] CreateRoleRequest request, CancellationToken ct)
    {
        var role = await _roleService.CreateAsync(request, ct);
        var wrappedRole = ApiResponse<RoleResponse>.Success(role, "Role created successfully.");
        return CreatedAtAction(nameof(GetById), new { id = role.Id }, wrappedRole);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<RoleResponse>>> Update(Guid id, [FromBody] UpdateRoleRequest request, CancellationToken ct)
    {
        var role = await _roleService.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<RoleResponse>.Success(role, "Role updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await _roleService.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Success(new { }, "Role deleted successfully."));
    }
}
