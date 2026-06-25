using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TruyenCV.Application.Modules.Role.DTOs.Requests;
using TruyenCV.Application.Modules.Role.DTOs.Responses;
using TruyenCV.Shared.Requests;
using TruyenCV.Shared.Responses;

namespace TruyenCV.Application.Modules.Role.Interfaces.Services;

public interface IRoleService
{
    Task<RoleResponse> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PaginatedResponse<RoleResponse>> GetPagedAsync(PaginationRequest request, CancellationToken ct = default);
    Task<RoleResponse> CreateAsync(CreateRoleRequest request, CancellationToken ct = default);
    Task<RoleResponse> UpdateAsync(Guid id, UpdateRoleRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
