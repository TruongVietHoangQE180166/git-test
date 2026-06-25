using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using TruyenCV.Application.Modules.Role.DTOs.Requests;
using TruyenCV.Application.Modules.Role.DTOs.Responses;
using TruyenCV.Application.Modules.Role.Interfaces.Services;
using TruyenCV.Shared.Exceptions;
using TruyenCV.Shared.Requests;
using TruyenCV.Shared.Responses;

namespace TruyenCV.Application.Common.Behaviors;

public class ValidationRoleService : IRoleService
{
    private readonly IRoleService _inner;
    private readonly IValidator<CreateRoleRequest> _createValidator;
    private readonly IValidator<UpdateRoleRequest> _updateValidator;
    private readonly IValidator<PaginationRequest> _paginationValidator;

    public ValidationRoleService(
        IRoleService inner,
        IValidator<CreateRoleRequest> createValidator,
        IValidator<UpdateRoleRequest> updateValidator,
        IValidator<PaginationRequest> paginationValidator)
    {
        _inner = inner;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _paginationValidator = paginationValidator;
    }

    public Task<RoleResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return _inner.GetByIdAsync(id, ct);
    }

    public async Task<PaginatedResponse<RoleResponse>> GetPagedAsync(PaginationRequest request, CancellationToken ct = default)
    {
        var result = await _paginationValidator.ValidateAsync(request, ct);
        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
            throw new Shared.Exceptions.ValidationException(errors);
        }
        return await _inner.GetPagedAsync(request, ct);
    }

    public async Task<RoleResponse> CreateAsync(CreateRoleRequest request, CancellationToken ct = default)
    {
        var result = await _createValidator.ValidateAsync(request, ct);
        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
            throw new Shared.Exceptions.ValidationException(errors);
        }
        return await _inner.CreateAsync(request, ct);
    }

    public async Task<RoleResponse> UpdateAsync(Guid id, UpdateRoleRequest request, CancellationToken ct = default)
    {
        var result = await _updateValidator.ValidateAsync(request, ct);
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
}
