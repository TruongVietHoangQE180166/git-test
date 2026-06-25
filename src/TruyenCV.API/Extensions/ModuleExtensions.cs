using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TruyenCV.Shared.Responses;

namespace TruyenCV.API.Extensions;

public static class ModuleExtensions
{
    public static IServiceCollection AddApiModules(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        // FluentValidation configuration
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        // Register validators from Application layer
        services.AddValidatorsFromAssembly(typeof(Application.Common.Interfaces.IRepository<>).Assembly);

        // Customize the default validation error response to match our ErrorResponse envelope
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value != null && e.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value != null
                            ? kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                            : []
                    );

                var errorResponse = ErrorResponse.Create(
                    statusCode: StatusCodes.Status400BadRequest,
                    message: "One or more validation errors occurred.",
                    traceId: context.HttpContext.TraceIdentifier,
                    errors: errors
                );

                return new BadRequestObjectResult(errorResponse);
            };
        });

        // Configure CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader());
        });

        return services;
    }
}
