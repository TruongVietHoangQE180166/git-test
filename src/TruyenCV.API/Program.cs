using System;
using System.IO;
using TruyenCV.API.Extensions;
using TruyenCV.API.Middleware;
using TruyenCV.Application.Extensions;
using TruyenCV.Infrastructure.Extensions;

// Load environment variables from .env file by searching upwards from the executing directory
var currentDir = new DirectoryInfo(AppContext.BaseDirectory);
while (currentDir != null)
{
    var envFilePath = Path.Combine(currentDir.FullName, ".env");
    if (File.Exists(envFilePath))
    {
        foreach (var line in File.ReadAllLines(envFilePath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

            var parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                var key = parts[0].Trim();
                var val = parts[1].Trim();
                if ((val.StartsWith("\"") && val.EndsWith("\"")) || (val.StartsWith("'") && val.EndsWith("'")))
                {
                    val = val[1..^1];
                }
                Environment.SetEnvironmentVariable(key, val);
            }
        }
        break;
    }
    currentDir = currentDir.Parent;
}

var builder = WebApplication.CreateBuilder(args);

// 1. Setup Logging (Serilog)
builder.AddLoggingConfiguration();

// 2. Setup Database & Infrastructure Services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 3. Setup Security (JWT, Authentication, Authorization)
builder.Services.AddSecurityConfiguration(builder.Configuration);

// 4. Setup Swagger / OpenAPI
builder.Services.AddSwaggerConfiguration();
builder.Services.AddEndpointsApiExplorer();

// 5. Setup Controllers, Validation, CORS
builder.Services.AddApiModules();

var app = builder.Build();

// ── HTTP Request Pipeline ────────────────────────────────────────────────

// Global Exception Handler
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerConfiguration();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Tự động chạy EF Core Migrations trước khi nhận Request
app.ApplyDatabaseMigrationsAsync().Wait();

app.Run();
