using System.Text;
using KafkaFlow;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedLibrary.SeriLogging.Extensions;

namespace SharedLibrary.KafkaFlow.SharedMiddlewares;

public sealed class CorrelationIIdLoggingMiddleware : IMessageMiddleware
{
    private const string CorrelationIdHeader = "CorrelationId";
    
    private readonly ILogger<CorrelationIIdLoggingMiddleware> _logger;

    public CorrelationIIdLoggingMiddleware(ILogger<CorrelationIIdLoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        // Attempt to extract the Correlation ID from the message headers
        var correlationId = !string.IsNullOrEmpty(context.Headers.GetString(CorrelationIdHeader)) ? 
            context.Headers.GetString(CorrelationIdHeader) : Guid.NewGuid().ToString();
        
        _logger.LogInfo($"Kafka Message Received | {CorrelationIdHeader}: {correlationId}");
        
        // Update or add the Correlation ID back to the message headers (optional)
        context.Headers[CorrelationIdHeader] = Encoding.UTF8.GetBytes(correlationId);

        // Add the Correlation ID to the Serilog LogContext
        using (LogContext.PushProperty(CorrelationIdHeader, correlationId))
        {
            await next(context); // Continue processing
        }
        
        // Log the completion of the message processing
        _logger.LogInfo($"Kafka Message Processed | {CorrelationIdHeader}: {correlationId}");

    }
}