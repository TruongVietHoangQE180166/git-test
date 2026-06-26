using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using TruyenCV.Application.Modules.User.DTOs.Requests;
using TruyenCV.Application.Modules.User.DTOs.Responses;
using TruyenCV.Application.Modules.User.Interfaces.Services;
using TruyenCV.Application.Modules.User.Interfaces.Repositories;
using TruyenCV.Application.Modules.Role.Interfaces.Repositories;
using TruyenCV.Domain.Enums;
using TruyenCV.Shared.Exceptions;
using TruyenCV.Shared.Requests;
using TruyenCV.Shared.Responses;

namespace TruyenCV.Application.Modules.User.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;

    public UserService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    public async Task<UserResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _userRepository.GetWithRoleAsync(id, ct);
        if (user == null || user.Status == EntityStatus.Deleted)
        {
            throw new NotFoundException($"User with ID {id} was not found.");
        }
        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByUsernameAsync(username, ct);
        if (user == null || user.Status == EntityStatus.Deleted)
        {
            throw new NotFoundException($"User with username {username} was not found.");
        }

        if (user.Role == null)
        {
            user.Role = await _roleRepository.GetByIdAsync(user.RoleId, ct) 
                ?? throw new NotFoundException($"Role with ID {user.RoleId} was not found.");
        }

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<PaginatedResponse<UserResponse>> GetPagedAsync(PaginationRequest request, CancellationToken ct = default)
    {
        var (users, totalCount) = await _userRepository.GetPagedAsync(
            request.PageNumber ?? 1,
            request.PageSize ?? 10,
            request.SortField,
            request.SortDirection,
            request.SearchTerm,
            ct);

        var mappedUsers = _mapper.Map<IReadOnlyList<UserResponse>>(users);
        return PaginatedResponse<UserResponse>.Create(mappedUsers, totalCount, request.PageNumber ?? 1, request.PageSize ?? 10);
    }

    public async Task<UserResponse> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default)
    {
        var user = await _userRepository.GetWithRoleAsync(id, ct);
        if (user == null || user.Status == EntityStatus.Deleted)
        {
            throw new NotFoundException($"User with ID {id} was not found.");
        }

        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (await _userRepository.ExistsByEmailAsync(request.Email, ct))
            {
                throw new ConflictException("Email is already in use.");
            }
            user.Email = request.Email;
        }

        if (!string.Equals(user.Username, request.Username, StringComparison.OrdinalIgnoreCase))
        {
            if (await _userRepository.ExistsByUsernameAsync(request.Username, ct))
            {
                throw new ConflictException("Username is already in use.");
            }
            user.Username = request.Username;
        }

        if (user.RoleId != request.RoleId)
        {
            var roleExists = await _roleRepository.ExistsByIdAsync(request.RoleId, ct);
            if (!roleExists)
            {
                throw new NotFoundException($"Role with ID {request.RoleId} was not found.");
            }
            user.RoleId = request.RoleId;
        }

        user.UpdatedAt = DateTime.UtcNow;
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(ct);

        var updatedUser = await _userRepository.GetWithRoleAsync(id, ct);
        return _mapper.Map<UserResponse>(updatedUser);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(id, ct);
        if (user == null || user.Status == EntityStatus.Deleted)
        {
            throw new NotFoundException($"User with ID {id} was not found.");
        }

        await _userRepository.SoftDeleteAsync(id, ct);
    }

    public async Task BanAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _userRepository.GetWithRoleAsync(id, ct);
        if (user == null || user.Status == EntityStatus.Deleted)
        {
            throw new NotFoundException($"User with ID {id} was not found.");
        }

        // Safety check: Prevent banning Admin accounts
        if (user.Role != null && string.Equals(user.Role.Name, TruyenCV.Shared.Constants.RoleConstants.Admin, StringComparison.OrdinalIgnoreCase))
        {
            throw new BusinessRuleException("Administrator accounts cannot be banned.");
        }

        await _userRepository.BanAsync(id, ct);
    }

    public async Task UnbanAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(id, ct);
        if (user == null || user.Status == EntityStatus.Deleted)
        {
            throw new NotFoundException($"User with ID {id} was not found.");
        }

        await _userRepository.UnbanAsync(id, ct);
    }
}
