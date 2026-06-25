using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using TruyenCV.Application.Modules.Auth.DTOs.Requests;
using TruyenCV.Application.Modules.Auth.DTOs.Responses;
using TruyenCV.Application.Modules.Auth.Interfaces.Services;
using TruyenCV.Shared.Exceptions;

namespace TruyenCV.Application.Common.Behaviors;

public class ValidationAuthService : IAuthService
{
    private readonly IAuthService _inner;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public ValidationAuthService(
        IAuthService inner,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator)
    {
        _inner = inner;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var result = await _registerValidator.ValidateAsync(request, ct);
        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
            throw new Shared.Exceptions.ValidationException(errors);
        }
        return await _inner.RegisterAsync(request, ct);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var result = await _loginValidator.ValidateAsync(request, ct);
        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
            throw new Shared.Exceptions.ValidationException(errors);
        }
        return await _inner.LoginAsync(request, ct);
    }

    public Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default)
    {
        return _inner.RefreshTokenAsync(request, ct);
    }

    public Task RevokeSessionAsync(string refreshToken, CancellationToken ct = default)
    {
        return _inner.RevokeSessionAsync(refreshToken, ct);
    }
}
