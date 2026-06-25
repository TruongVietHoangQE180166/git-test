using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using TruyenCV.API.Configurations;
using TruyenCV.Shared.Constants;
using TruyenCV.Shared.Responses;

namespace TruyenCV.API.Extensions;

public static class SecurityExtensions
{
    public static IServiceCollection AddSecurityConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = new JwtOptions();
        configuration.GetSection(JwtOptions.SectionName).Bind(jwtOptions);

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    // Skip the default behavior of returning empty 401 response
                    context.HandleResponse();

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var errorResponse = ErrorResponse.Create(
                        statusCode: StatusCodes.Status401Unauthorized,
                        message: string.IsNullOrEmpty(context.ErrorDescription)
                            ? "You are not authorized to access this resource."
                            : context.ErrorDescription,
                        traceId: context.HttpContext.TraceIdentifier
                    );

                    var result = System.Text.Json.JsonSerializer.Serialize(
                        errorResponse,
                        new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase }
                    );

                    await context.Response.WriteAsync(result);
                },
                OnForbidden = async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";

                    var errorResponse = ErrorResponse.Create(
                        statusCode: StatusCodes.Status403Forbidden,
                        message: "You do not have permission to access this resource.",
                        traceId: context.HttpContext.TraceIdentifier
                    );

                    var result = System.Text.Json.JsonSerializer.Serialize(
                        errorResponse,
                        new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase }
                    );

                    await context.Response.WriteAsync(result);
                }
            };
        });

        services.AddAuthorizationBuilder()
            .AddPolicy(AuthPolicies.RequireAdminRole, policy => policy.RequireRole(RoleConstants.Admin))
            .AddPolicy(AuthPolicies.RequireModeratorRole, policy => policy.RequireRole(RoleConstants.Moderator, RoleConstants.Admin));

        return services;
    }
}
