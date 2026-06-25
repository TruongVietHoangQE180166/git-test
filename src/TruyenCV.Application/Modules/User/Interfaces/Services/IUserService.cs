using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TruyenCV.Application.Modules.User.DTOs.Requests;
using TruyenCV.Application.Modules.User.DTOs.Responses;
using TruyenCV.Shared.Requests;
using TruyenCV.Shared.Responses;

namespace TruyenCV.Application.Modules.User.Interfaces.Services;

public interface IUserService
{
    Task<UserResponse> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<UserResponse> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<PaginatedResponse<UserResponse>> GetPagedAsync(PaginationRequest request, CancellationToken ct = default);
    Task<UserResponse> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
