using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TruyenCV.Shared.Exceptions;

namespace TruyenCV.Application.Common.Behaviors;

public class LoggingProxy<T> : DispatchProxy
{
    private T _target = default!;
    private ILogger _logger = default!;

    public static T Create(T target, ILogger logger)
    {
        object? proxy = Create<T, LoggingProxy<T>>();
        if (proxy == null)
        {
            throw new InvalidOperationException("Failed to create dispatch proxy.");
        }
        ((LoggingProxy<T>)proxy)._target = target;
        ((LoggingProxy<T>)proxy)._logger = logger;
        return (T)proxy;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod == null) return null;

        var methodName = targetMethod.Name;
        var serviceName = typeof(T).Name;

        _logger.LogInformation("Starting execution of service method {Service}.{Method}", serviceName, methodName);
        
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = targetMethod.Invoke(_target, args);

            if (result != null && targetMethod.ReturnType.IsGenericType && targetMethod.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var resultType = targetMethod.ReturnType.GetGenericArguments()[0];
                var method = typeof(LoggingProxy<T>)
                    .GetMethod(nameof(ConvertGenericTask), BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.MakeGenericMethod(resultType);
                
                return method?.Invoke(this, [result, serviceName, methodName, stopwatch]);
            }
            else if (result is Task task)
            {
                return ConvertTask(task, serviceName, methodName, stopwatch);
            }

            stopwatch.Stop();
            _logger.LogInformation("Successfully completed service method {Service}.{Method} in {Elapsed}ms", serviceName, methodName, stopwatch.ElapsedMilliseconds);
            return result;
        }
        catch (TargetInvocationException ex)
        {
            stopwatch.Stop();
            var innerEx = ex.InnerException ?? ex;
            if (innerEx.IsSystemError())
            {
                _logger.LogError(innerEx, "Service method {Service}.{Method} failed after {Elapsed}ms", serviceName, methodName, stopwatch.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogInformation("Service method {Service}.{Method} returned business error: {Message} in {Elapsed}ms", serviceName, methodName, innerEx.Message, stopwatch.ElapsedMilliseconds);
            }
            throw innerEx;
        }
    }

    private async Task ConvertTask(Task task, string serviceName, string methodName, Stopwatch stopwatch)
    {
        try
        {
            await task;
            stopwatch.Stop();
            _logger.LogInformation("Successfully completed async service method {Service}.{Method} in {Elapsed}ms", serviceName, methodName, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            if (ex.IsSystemError())
            {
                _logger.LogError(ex, "Async service method {Service}.{Method} failed after {Elapsed}ms", serviceName, methodName, stopwatch.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogInformation("Async service method {Service}.{Method} returned business error: {Message} in {Elapsed}ms", serviceName, methodName, ex.Message, stopwatch.ElapsedMilliseconds);
            }
            throw;
        }
    }

    private async Task<TResult> ConvertGenericTask<TResult>(Task<TResult> task, string serviceName, string methodName, Stopwatch stopwatch)
    {
        try
        {
            var result = await task;
            stopwatch.Stop();
            _logger.LogInformation("Successfully completed async service method {Service}.{Method} in {Elapsed}ms", serviceName, methodName, stopwatch.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            if (ex.IsSystemError())
            {
                _logger.LogError(ex, "Async service method {Service}.{Method} failed after {Elapsed}ms", serviceName, methodName, stopwatch.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogInformation("Async service method {Service}.{Method} returned business error: {Message} in {Elapsed}ms", serviceName, methodName, ex.Message, stopwatch.ElapsedMilliseconds);
            }
            throw;
        }
    }
}
