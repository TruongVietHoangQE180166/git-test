using System;
using System.IO;
using TruyenCV.API.Extensions;
using TruyenCV.API.Middleware;
using TruyenCV.Application.Extensions;
using TruyenCV.Infrastructure.Extensions;

// Load environment variables from .env file by searching upwards from CurrentDirectory and BaseDirectory
var searchPaths = new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory };
foreach (var path in searchPaths)
{
    var currentDir = new DirectoryInfo(path);
    var found = false;
    while (currentDir != null)
    {
        var envFilePath = Path.Combine(currentDir.FullName, ".env");
        if (File.Exists(envFilePath))
        {
            var lines = File.ReadAllLines(envFilePath);
            string? currentKey = null;
            var currentValue = new System.Text.StringBuilder();
            var inQuotes = false;

            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (!inQuotes)
                {
                    if (line.StartsWith("#")) continue;

                    var parts = line.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var val = parts[1].Trim();

                        // Check if value starts a multiline double-quoted string
                        if (val.StartsWith("\"") && !val.EndsWith("\"") && val.Split('"').Length % 2 == 0)
                        {
                            inQuotes = true;
                            currentKey = key;
                            currentValue.Clear();
                            currentValue.Append(val.Substring(1)); // strip starting quote
                        }
                        else
                        {
                            // Strip quotes if they exist on a single line
                            if ((val.StartsWith("\"") && val.EndsWith("\"")) || (val.StartsWith("'") && val.EndsWith("'")))
                            {
                                val = val[1..^1];
                            }
                            Environment.SetEnvironmentVariable(key, val);
                        }
                    }
                }
                else
                {
                    // Inside multiline quotes - check if this line closes the quote
                    if (line.EndsWith("\"") && line.Split('"').Length % 2 == 0)
                    {
                        inQuotes = false;
                        currentValue.Append(" ").Append(line.Substring(0, line.Length - 1)); // strip ending quote
                        if (currentKey != null)
                        {
                            Environment.SetEnvironmentVariable(currentKey, currentValue.ToString().Trim());
                        }
                    }
                    else
                    {
                        currentValue.Append(" ").Append(line);
                    }
                }
            }
            found = true;
            break;
        }
        currentDir = currentDir.Parent;
    }
    if (found) break;
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

// Correlation ID & Request Tracing
app.UseMiddleware<CorrelationIdMiddleware>();

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
