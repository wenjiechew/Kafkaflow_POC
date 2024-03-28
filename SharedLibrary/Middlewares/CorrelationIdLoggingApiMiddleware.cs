using Microsoft.AspNetCore.Http;
using SharedLibrary.Core.Constants;

namespace SharedLibrary.Middlewares;

public class CorrelationIdLoggingApiMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdLoggingApiMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // Check if the incoming request has a correlation ID
        if (!context.Request.Headers.TryGetValue(GlobalConstants.CorrelationIdHeader, out var correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
            context.Request.Headers.Append(GlobalConstants.CorrelationIdHeader, correlationId);
        }

        // Make the correlation ID available for the duration of this request
        context.Items[GlobalConstants.CorrelationIdHeader] = correlationId;

        // Proceed with the request pipeline
        await _next(context);

        // Optionally, add the correlation ID to the response headers for the client
        context.Response.Headers[GlobalConstants.CorrelationIdHeader] = correlationId;
    }
}