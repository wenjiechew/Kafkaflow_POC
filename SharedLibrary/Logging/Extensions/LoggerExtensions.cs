using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace SharedLibrary.Logging.Extensions;

public static class LoggerExtensions
{
    public static void LogInfo(this ILogger logger,
        string message,
        [CallerMemberName] string memberName = "")
    {
        // Enrich log with class and member name, then log the information
        logger.LogInformation("[{MemberName}] - {Message}", memberName, message);
    }
    
    public static void LogError(this ILogger logger,
        string message,
        [CallerMemberName] string memberName = "")
    {
        // Enrich log with class and member name, then log the error
        logger.LogError("[{MemberName}] - {Message}", memberName, message);
    }
    
    public static void LogWarning(this ILogger logger,
        string message,
        [CallerMemberName] string memberName = "")
    {
        // Enrich log with class and member name, then log the warning
        logger.LogWarning("[{MemberName}] - {Message}", memberName, message);
    }
    
    public static void LogDebug(this ILogger logger,
        string message,
        [CallerMemberName] string memberName = "")
    {
        // Enrich log with class and member name, then log the debug
        logger.LogDebug("[{MemberName}] - {Message}", memberName, message);
    }
    
    public static void LogCritical(this ILogger logger,
        string message,
        [CallerMemberName] string memberName = "")
    {
        // Enrich log with class and member name, then log the critical
        logger.LogCritical("[{MemberName}] - {Message}", memberName, message);
    }
    
    public static void LogTrace(this ILogger logger,
        string message,
        [CallerMemberName] string memberName = "")
    {
        // Enrich log with class and member name, then log the trace
        logger.LogTrace("[{MemberName}] - {Message}", memberName, message);
    }
    
    public static void LogFatal(this ILogger logger,
        string message,
        [CallerMemberName] string memberName = "")
    {
        // Enrich log with class and member name, then log the information
        logger.LogInformation("[{MemberName}] - {Message}", memberName, message);
    }
    
    
    
    

    
    

}