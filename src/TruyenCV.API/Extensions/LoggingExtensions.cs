using Serilog;

namespace TruyenCV.API.Extensions;

public static class LoggingExtensions
{
    public static WebApplicationBuilder AddLoggingConfiguration(this WebApplicationBuilder builder)
    {
        // Configure Serilog
        const string outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}]{ReqId} {Message:lj}{NewLine}{Exception}";

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: outputTemplate)
            .WriteTo.File(
                "Logs/truyencv-log-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: outputTemplate)
            .CreateLogger();

        builder.Host.UseSerilog();

        return builder;
    }
}
