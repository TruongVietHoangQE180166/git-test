using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using TruyenCV.Application.Modules.User.DTOs.Requests;
using TruyenCV.Application.Modules.User.DTOs.Responses;
using TruyenCV.Application.Modules.User.Interfaces.Services;
using TruyenCV.Shared.Exceptions;
using TruyenCV.Shared.Requests;
using TruyenCV.Shared.Responses;

namespace TruyenCV.Application.Common.Behaviors;

public class ValidationUserService : IUserService
{
    private readonly IUserService _inner;
    private readonly IValidator<UpdateUserRequest> _updateUserValidator;
    private readonly IValidator<PaginationRequest> _paginationValidator;

    public ValidationUserService(
        IUserService inner, 
        IValidator<UpdateUserRequest> updateUserValidator,
        IValidator<PaginationRequest> paginationValidator)
    {
        _inner = inner;
        _updateUserValidator = updateUserValidator;
        _paginationValidator = paginationValidator;
    }

    public Task<UserResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return _inner.GetByIdAsync(id, ct);
    }

    public Task<UserResponse> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        return _inner.GetByUsernameAsync(username, ct);
    }

    public async Task<PaginatedResponse<UserResponse>> GetPagedAsync(PaginationRequest request, CancellationToken ct = default)
    {
        var result = await _paginationValidator.ValidateAsync(request, ct);
        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
            throw new Shared.Exceptions.ValidationException(errors);
        }
        return await _inner.GetPagedAsync(request, ct);
    }

    public async Task<UserResponse> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default)
    {
        var result = await _updateUserValidator.ValidateAsync(request, ct);
        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
            throw new Shared.Exceptions.ValidationException(errors);
        }
        return await _inner.UpdateAsync(id, request, ct);
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        return _inner.DeleteAsync(id, ct);
    }

    public Task BanAsync(Guid id, CancellationToken ct = default)
    {
        return _inner.BanAsync(id, ct);
    }

    public Task UnbanAsync(Guid id, CancellationToken ct = default)
    {
        return _inner.UnbanAsync(id, ct);
    }
}
