using KafkaFlow;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedLibrary.Core.Constants;
using SharedLibrary.Logging.Extensions;
using System.Text;

namespace SharedLibrary.KafkaFlow.SharedMiddlewares;

public sealed class CorrelationIdLoggingKafkaFlowMiddleware : IMessageMiddleware
{
    private readonly ILogger<CorrelationIdLoggingKafkaFlowMiddleware> _logger;

    public CorrelationIdLoggingKafkaFlowMiddleware(ILogger<CorrelationIdLoggingKafkaFlowMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        // Attempt to extract the Correlation ID from the message headers
        var correlationId = !string.IsNullOrEmpty(context.Headers.GetString(GlobalConstants.CorrelationIdHeader)) ?
            context.Headers.GetString(GlobalConstants.CorrelationIdHeader) : Guid.NewGuid().ToString();

        _logger.LogInfo($"Kafka Message Received | {GlobalConstants.CorrelationIdHeader}: {correlationId}");

        // Update or add the Correlation ID back to the message headers (optional)
        context.Headers[GlobalConstants.CorrelationIdHeader] = Encoding.UTF8.GetBytes(correlationId);

        // Add the Correlation ID to the Serilog LogContext
        using (LogContext.PushProperty(GlobalConstants.CorrelationIdHeader, correlationId))
        {
            await next(context); // Continue processing
        }

        // Log the completion of the message processing
        _logger.LogInfo($"Kafka Message Processed | {GlobalConstants.CorrelationIdHeader}: {correlationId}");

    }
}