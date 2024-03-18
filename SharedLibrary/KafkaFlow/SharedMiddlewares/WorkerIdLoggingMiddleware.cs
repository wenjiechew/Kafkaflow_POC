using KafkaFlow;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;
using SharedLibrary.SeriLogging.Extensions;

namespace SharedLibrary.KafkaFlow.SharedMiddlewares;

public sealed class WorkerIdLoggingMiddleware : IMessageMiddleware
{
    private readonly ILogger<WorkerIdLoggingMiddleware> _logger;

    public WorkerIdLoggingMiddleware(ILogger<WorkerIdLoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        var workerId = context.ConsumerContext.WorkerId;
        
        _logger.LogInfo($"Processing with WorkerId: {workerId}");
        if (workerId > 0)
        {
            using (LogContext.PushProperty("WorkerId", workerId))
            {
                await next(context);
            }
        }
        else
        {
            await next(context);
        }
    }
}