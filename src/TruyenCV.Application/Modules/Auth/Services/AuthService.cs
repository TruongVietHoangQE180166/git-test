using AutoMapper;
using TruyenCV.Application.Common.Interfaces;
using TruyenCV.Application.Modules.Auth.DTOs.Requests;
using TruyenCV.Application.Modules.Auth.DTOs.Responses;
using TruyenCV.Application.Modules.Auth.Interfaces.Repositories;
using TruyenCV.Application.Modules.Auth.Interfaces.Services;
using TruyenCV.Application.Modules.Role.Interfaces.Repositories;
using TruyenCV.Application.Modules.User.Interfaces.Repositories;
using TruyenCV.Domain.Entities;
using TruyenCV.Shared.Constants;
using TruyenCV.Shared.Exceptions;

namespace TruyenCV.Application.Modules.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IAuthRepository _authRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly IMapper _mapper;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IAuthRepository authRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _authRepository = authRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _mapper = mapper;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email, ct))
        {
            throw new ConflictException("Email is already in use.");
        }

        if (await _userRepository.ExistsByUsernameAsync(request.Username, ct))
        {
            throw new ConflictException("Username is already in use.");
        }

        var userRole = await _roleRepository.GetByNameAsync(RoleConstants.User, ct);
        if (userRole == null)
        {
            throw new BusinessRuleException("Default user role not found in the system.");
        }

        var user = new TruyenCV.Domain.Entities.User
        {
            Email = request.Email,
            Username = request.Username,
            PasswordHash = _passwordHasher.Hash(request.Password),
            RoleId = userRole.Id,
            Role = userRole,
            Profile = new TruyenCV.Domain.Entities.Profile
            {
                FullName = request.FullName,
                Slug = request.Username.ToLowerInvariant(),
                IsPublic = true
            }
        };

        await _userRepository.AddAsync(user, ct);
        await _userRepository.SaveChangesAsync(ct);

        return await GenerateAuthResponseAsync(user, ct);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, ct);
        if (user == null)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        // Must load role for JWT generation
        var userWithRole = await _userRepository.GetWithRoleAsync(user.Id, ct);
        if (userWithRole == null) throw new UnauthorizedException("User not found.");

        return await GenerateAuthResponseAsync(userWithRole, ct);
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default)
    {
        var session = await _authRepository.GetByRefreshTokenAsync(request.RefreshToken, ct);

        if (session == null || session.IsRevoked || session.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedException("Invalid or expired refresh token.");
        }

        // Revoke the old token (Refresh Token Rotation)
        session.IsRevoked = true;
        _authRepository.Update(session);

        var userWithRole = await _userRepository.GetWithRoleAsync(session.UserId, ct);
        if (userWithRole == null) throw new UnauthorizedException("User not found.");

        return await GenerateAuthResponseAsync(userWithRole, ct);
    }

    public async Task RevokeSessionAsync(string refreshToken, CancellationToken ct = default)
    {
        await _authRepository.RevokeSessionAsync(refreshToken, ct);
        await _authRepository.SaveChangesAsync(ct);
    }

    private async Task<AuthResponse> GenerateAuthResponseAsync(TruyenCV.Domain.Entities.User user, CancellationToken ct)
    {
        var accessToken = _jwtProvider.GenerateAccessToken(user);
        var refreshToken = _jwtProvider.GenerateRefreshToken();

        var authSession = new AuthSession
        {
            UserId = user.Id,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7) // 7 days lifetime for refresh token
        };

        await _authRepository.AddAsync(authSession, ct);
        await _authRepository.SaveChangesAsync(ct);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = _mapper.Map<AuthResponse.UserDto>(user)
        };
    }
}
