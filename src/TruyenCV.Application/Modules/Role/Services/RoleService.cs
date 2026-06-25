using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using TruyenCV.Application.Modules.Role.DTOs.Requests;
using TruyenCV.Application.Modules.Role.DTOs.Responses;
using TruyenCV.Application.Modules.Role.Interfaces.Services;
using TruyenCV.Application.Modules.Role.Interfaces.Repositories;
using TruyenCV.Domain.Entities;
using TruyenCV.Domain.Enums;
using TruyenCV.Shared.Constants;
using TruyenCV.Shared.Exceptions;
using TruyenCV.Shared.Requests;
using TruyenCV.Shared.Responses;

namespace TruyenCV.Application.Modules.Role.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;

    public RoleService(
        IRoleRepository roleRepository,
        IMapper mapper)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    public async Task<RoleResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var rolesWithCount = await _roleRepository.GetAllWithUserCountAsync(ct);
        var match = rolesWithCount.FirstOrDefault(r => r.Role.Id == id);
        if (match.Role == null || match.Role.Status == EntityStatus.Deleted)
        {
            throw new NotFoundException($"Role with ID {id} was not found.");
        }

        var response = _mapper.Map<RoleResponse>(match.Role);
        response.UserCount = match.UserCount;
        return response;
    }

    public async Task<PaginatedResponse<RoleResponse>> GetPagedAsync(PaginationRequest request, CancellationToken ct = default)
    {
        var rolesWithCount = await _roleRepository.GetAllWithUserCountAsync(ct);
        var activeRoles = rolesWithCount.Where(r => r.Role.Status != EntityStatus.Deleted);

        // Searching
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            activeRoles = activeRoles.Where(r => r.Role.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        // Sorting
        var isDescending = string.Equals(request.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        activeRoles = request.SortField?.ToLowerInvariant() switch
        {
            "name" => isDescending ? activeRoles.OrderByDescending(r => r.Role.Name) : activeRoles.OrderBy(r => r.Role.Name),
            "createdat" => isDescending ? activeRoles.OrderByDescending(r => r.Role.CreatedAt) : activeRoles.OrderBy(r => r.Role.CreatedAt),
            _ => activeRoles.OrderBy(r => r.Role.Name)
        };

        var totalCount = activeRoles.Count();
        var pageNum = request.PageNumber ?? 1;
        var pageSize = request.PageSize ?? 10;

        var pagedItems = activeRoles
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var responses = pagedItems.Select(item =>
        {
            var res = _mapper.Map<RoleResponse>(item.Role);
            res.UserCount = item.UserCount;
            return res;
        }).ToList();

        return PaginatedResponse<RoleResponse>.Create(responses, totalCount, pageNum, pageSize);
    }

    public async Task<RoleResponse> CreateAsync(CreateRoleRequest request, CancellationToken ct = default)
    {
        if (await _roleRepository.ExistsByNameAsync(request.Name, ct))
        {
            throw new ConflictException($"Role name '{request.Name}' is already in use.");
        }

        var role = new Domain.Entities.Role
        {
            Name = request.Name
        };

        await _roleRepository.AddAsync(role, ct);
        await _roleRepository.SaveChangesAsync(ct);

        var response = _mapper.Map<RoleResponse>(role);
        response.UserCount = 0;
        return response;
    }

    public async Task<RoleResponse> UpdateAsync(Guid id, UpdateRoleRequest request, CancellationToken ct = default)
    {
        var role = await _roleRepository.GetByIdAsync(id, ct);
        if (role == null || role.Status == EntityStatus.Deleted)
        {
            throw new NotFoundException($"Role with ID {id} was not found.");
        }

        // Prevent modification of system roles
        if (RoleConstants.All.Contains(role.Name, StringComparer.OrdinalIgnoreCase))
        {
            throw new BusinessRuleException($"System role '{role.Name}' cannot be updated.");
        }

        if (!string.Equals(role.Name, request.Name, StringComparison.OrdinalIgnoreCase))
        {
            if (await _roleRepository.ExistsByNameAsync(request.Name, ct))
            {
                throw new ConflictException($"Role name '{request.Name}' is already in use.");
            }
            role.Name = request.Name;
        }

        role.UpdatedAt = DateTime.UtcNow;
        _roleRepository.Update(role);
        await _roleRepository.SaveChangesAsync(ct);

        var rolesWithCount = await _roleRepository.GetAllWithUserCountAsync(ct);
        var match = rolesWithCount.First(r => r.Role.Id == id);

        var response = _mapper.Map<RoleResponse>(match.Role);
        response.UserCount = match.UserCount;
        return response;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var rolesWithCount = await _roleRepository.GetAllWithUserCountAsync(ct);
        var match = rolesWithCount.FirstOrDefault(r => r.Role.Id == id);
        if (match.Role == null || match.Role.Status == EntityStatus.Deleted)
        {
            throw new NotFoundException($"Role with ID {id} was not found.");
        }

        var role = match.Role;

        // Prevent deletion of system roles
        if (RoleConstants.All.Contains(role.Name, StringComparer.OrdinalIgnoreCase))
        {
            throw new BusinessRuleException($"System role '{role.Name}' cannot be deleted.");
        }

        // Check user count
        if (match.UserCount > 0)
        {
            throw new BusinessRuleException($"Role '{role.Name}' cannot be deleted because it is assigned to {match.UserCount} users.");
        }

        role.Status = EntityStatus.Deleted;
        role.UpdatedAt = DateTime.UtcNow;
        _roleRepository.Update(role);
        await _roleRepository.SaveChangesAsync(ct);
    }
}
