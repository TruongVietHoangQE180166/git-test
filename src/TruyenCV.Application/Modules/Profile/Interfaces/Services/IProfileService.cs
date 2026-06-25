using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TruyenCV.Application.Modules.Profile.DTOs.Requests;
using TruyenCV.Application.Modules.Profile.DTOs.Responses;
using TruyenCV.Shared.Requests;
using TruyenCV.Shared.Responses;

namespace TruyenCV.Application.Modules.Profile.Interfaces.Services;

public interface IProfileService
{
    Task<ProfileResponse> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<ProfileResponse> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<PaginatedResponse<ProfileResponse>> GetPagedAsync(PaginationRequest request, CancellationToken ct = default);
    Task<ProfileResponse> UpdateAsync(Guid userId, UpdateProfileRequest request, CancellationToken ct = default);
}
