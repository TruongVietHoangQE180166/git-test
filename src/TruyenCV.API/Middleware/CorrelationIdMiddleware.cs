using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System;
using System.Threading.Tasks;

namespace TruyenCV.API.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationHeaderKey = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string correlationId;

        if (context.Request.Headers.TryGetValue(CorrelationHeaderKey, out var headerValue) && !string.IsNullOrWhiteSpace(headerValue))
        {
            correlationId = headerValue.ToString();
        }
        else if (context.Request.Headers.TryGetValue("X-Request-ID", out var requestHeaderValue) && !string.IsNullOrWhiteSpace(requestHeaderValue))
        {
            correlationId = requestHeaderValue.ToString();
        }
        else
        {
            correlationId = Guid.NewGuid().ToString("N")[..8];
        }

        // Set response header
        context.Response.OnStarting(() =>
        {
            if (!context.Response.Headers.ContainsKey(CorrelationHeaderKey))
            {
                context.Response.Headers[CorrelationHeaderKey] = correlationId;
            }
            return Task.CompletedTask;
        });

        // Sync with ASP.NET Core TraceIdentifier
        context.TraceIdentifier = correlationId;

        // Enrich Serilog logging context
        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("ReqId", $" [ReqId:{correlationId}]"))
        {
            await _next(context);
        }
    }
}
