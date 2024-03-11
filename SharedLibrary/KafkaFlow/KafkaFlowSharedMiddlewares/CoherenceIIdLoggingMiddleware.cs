using System.Text;
using KafkaFlow;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace SharedLibrary.KafkaFlow.KafkaFlowSharedMiddlewares;

public class CoherenceIIdLoggingMiddleware : IMessageMiddleware
{
    private readonly ILogger<CoherenceIIdLoggingMiddleware> _logger;

    public CoherenceIIdLoggingMiddleware(ILogger<CoherenceIIdLoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        var workerId = context.ConsumerContext.WorkerId;
        // Attempt to extract the Coherence ID from the message headers
        var coherenceId = string.IsNullOrEmpty(context.Headers.GetString("CoherenceId")) ? Guid.NewGuid().ToString() : context.Headers.GetString("CoherenceId");
        var messageId = string.IsNullOrEmpty(context.Headers.GetString("MessageId")) ? string.Empty : context.Headers.GetString("MessageId");
        _logger.LogInformation($"Kafka Message Received | MessageId: {messageId}| CoherenceId: {coherenceId}");
        // Update or add the Coherence ID back to the message headers (optional)
        context.Headers["CoherenceId"] = Encoding.UTF8.GetBytes(coherenceId);

        // Add the Coherence ID to the Serilog LogContext
        using (LogContext.PushProperty("CoherenceId", coherenceId))
        using (LogContext.PushProperty("MessageId", messageId))
        using (LogContext.PushProperty("WorkerId", workerId))
        {
            await next(context); // Continue processing
        }

    }
}