using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TruyenCV.Application.Common.Behaviors;
using TruyenCV.Application.Common.Interfaces;
using TruyenCV.Application.Modules.Auth.Interfaces.Repositories;
using TruyenCV.Application.Modules.Profile.Interfaces.Repositories;
using TruyenCV.Application.Modules.Role.Interfaces.Repositories;
using TruyenCV.Application.Modules.User.Interfaces.Repositories;
using TruyenCV.Infrastructure.Modules.Auth.Repositories;
using TruyenCV.Infrastructure.Modules.Profile.Repositories;
using TruyenCV.Infrastructure.Modules.Role.Repositories;
using TruyenCV.Infrastructure.Modules.User.Repositories;
using TruyenCV.Infrastructure.Security;
using TruyenCV.Infrastructure.Services;

namespace TruyenCV.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        // Core Repository implementations
        services.AddScoped<AuthRepository>();
        services.AddScoped<UserRepository>();
        services.AddScoped<ProfileRepository>();
        services.AddScoped<RoleRepository>();

        // Repositories wrapped with Logging behavior
        services.AddScoped<IAuthRepository>(provider => 
            LoggingProxy<IAuthRepository>.Create(
                provider.GetRequiredService<AuthRepository>(), 
                provider.GetRequiredService<ILogger<IAuthRepository>>()
            ));

        services.AddScoped<IUserRepository>(provider => 
            LoggingProxy<IUserRepository>.Create(
                provider.GetRequiredService<UserRepository>(), 
                provider.GetRequiredService<ILogger<IUserRepository>>()
            ));

        services.AddScoped<IProfileRepository>(provider => 
            LoggingProxy<IProfileRepository>.Create(
                provider.GetRequiredService<ProfileRepository>(), 
                provider.GetRequiredService<ILogger<IProfileRepository>>()
            ));

        services.AddScoped<IRoleRepository>(provider => 
            LoggingProxy<IRoleRepository>.Create(
                provider.GetRequiredService<RoleRepository>(), 
                provider.GetRequiredService<ILogger<IRoleRepository>>()
            ));

        // Security
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Core Service implementations
        services.AddScoped<EmailService>();
        services.AddScoped<CloudinaryService>();

        // Services wrapped with Logging behavior
        services.AddScoped<IEmailService>(provider => 
            LoggingProxy<IEmailService>.Create(
                provider.GetRequiredService<EmailService>(), 
                provider.GetRequiredService<ILogger<IEmailService>>()
            ));

        services.AddScoped<ICloudinaryService>(provider => 
            LoggingProxy<ICloudinaryService>.Create(
                provider.GetRequiredService<CloudinaryService>(), 
                provider.GetRequiredService<ILogger<ICloudinaryService>>()
            ));

        return services;
    }
}
