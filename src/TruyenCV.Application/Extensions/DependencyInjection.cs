using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using TruyenCV.Application.Common.Behaviors;
using TruyenCV.Application.Modules.Auth.DTOs.Requests;
using TruyenCV.Application.Modules.Auth.Interfaces.Services;
using TruyenCV.Application.Modules.Auth.Services;
using TruyenCV.Application.Modules.User.DTOs.Requests;
using TruyenCV.Application.Modules.User.Interfaces.Services;
using TruyenCV.Application.Modules.User.Services;
using TruyenCV.Application.Modules.Role.DTOs.Requests;
using TruyenCV.Application.Modules.Role.Interfaces.Services;
using TruyenCV.Application.Modules.Role.Services;
using TruyenCV.Application.Modules.Profile.DTOs.Requests;
using TruyenCV.Application.Modules.Profile.Interfaces.Services;
using TruyenCV.Application.Modules.Profile.Services;

namespace TruyenCV.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(DependencyInjection).Assembly));

        // Core implementations
        services.AddScoped<AuthService>();
        services.AddScoped<UserService>();
        services.AddScoped<RoleService>();
        services.AddScoped<ProfileService>();

        // Decorators for validation & logging behaviors
        services.AddScoped<IAuthService>(provider => 
        {
            var validationService = new ValidationAuthService(
                provider.GetRequiredService<AuthService>(),
                provider.GetRequiredService<IValidator<RegisterRequest>>(),
                provider.GetRequiredService<IValidator<LoginRequest>>()
            );
            return LoggingProxy<IAuthService>.Create(
                validationService, 
                provider.GetRequiredService<ILogger<IAuthService>>()
            );
        });

        services.AddScoped<IUserService>(provider => 
        {
            var validationService = new ValidationUserService(
                provider.GetRequiredService<UserService>(),
                provider.GetRequiredService<IValidator<UpdateUserRequest>>(),
                provider.GetRequiredService<IValidator<Shared.Requests.PaginationRequest>>()
            );
            return LoggingProxy<IUserService>.Create(
                validationService, 
                provider.GetRequiredService<ILogger<IUserService>>()
            );
        });

        services.AddScoped<IRoleService>(provider => 
        {
            var validationService = new ValidationRoleService(
                provider.GetRequiredService<RoleService>(),
                provider.GetRequiredService<IValidator<CreateRoleRequest>>(),
                provider.GetRequiredService<IValidator<UpdateRoleRequest>>(),
                provider.GetRequiredService<IValidator<Shared.Requests.PaginationRequest>>()
            );
            return LoggingProxy<IRoleService>.Create(
                validationService, 
                provider.GetRequiredService<ILogger<IRoleService>>()
            );
        });

        services.AddScoped<IProfileService>(provider => 
        {
            var validationService = new ValidationProfileService(
                provider.GetRequiredService<ProfileService>(),
                provider.GetRequiredService<IValidator<UpdateProfileRequest>>(),
                provider.GetRequiredService<IValidator<Shared.Requests.PaginationRequest>>()
            );
            return LoggingProxy<IProfileService>.Create(
                validationService, 
                provider.GetRequiredService<ILogger<IProfileService>>()
            );
        });

        return services;
    }
}
