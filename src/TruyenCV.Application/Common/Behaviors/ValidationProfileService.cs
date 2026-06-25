using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using TruyenCV.Application.Modules.Profile.DTOs.Requests;
using TruyenCV.Application.Modules.Profile.DTOs.Responses;
using TruyenCV.Application.Modules.Profile.Interfaces.Services;
using TruyenCV.Shared.Exceptions;
using TruyenCV.Shared.Requests;
using TruyenCV.Shared.Responses;

namespace TruyenCV.Application.Common.Behaviors;

public class ValidationProfileService : IProfileService
{
    private readonly IProfileService _inner;
    private readonly IValidator<UpdateProfileRequest> _updateValidator;
    private readonly IValidator<PaginationRequest> _paginationValidator;

    public ValidationProfileService(
        IProfileService inner, 
        IValidator<UpdateProfileRequest> updateValidator,
        IValidator<PaginationRequest> paginationValidator)
    {
        _inner = inner;
        _updateValidator = updateValidator;
        _paginationValidator = paginationValidator;
    }

    public Task<ProfileResponse> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return _inner.GetByUserIdAsync(userId, ct);
    }

    public Task<ProfileResponse> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        return _inner.GetBySlugAsync(slug, ct);
    }

    public async Task<PaginatedResponse<ProfileResponse>> GetPagedAsync(PaginationRequest request, CancellationToken ct = default)
    {
        var result = await _paginationValidator.ValidateAsync(request, ct);
        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
            throw new Shared.Exceptions.ValidationException(errors);
        }
        return await _inner.GetPagedAsync(request, ct);
    }

    public async Task<ProfileResponse> UpdateAsync(Guid userId, UpdateProfileRequest request, CancellationToken ct = default)
    {
        var result = await _updateValidator.ValidateAsync(request, ct);
        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
            throw new Shared.Exceptions.ValidationException(errors);
        }
        return await _inner.UpdateAsync(userId, request, ct);
    }
}
