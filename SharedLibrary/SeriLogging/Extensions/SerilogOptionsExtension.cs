using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using SharedLibrary.SeriLogging.Configurations;

namespace SharedLibrary.SeriLogging.Extensions;

public static class SerilogOptionsExtension
{
    public static LoggerConfiguration ConfigureFileSink(this LoggerConfiguration loggerConfiguration, IConfigurationSection fileSinkConfigSection)
    {
        var fileSinkConfig = fileSinkConfigSection.Get<SerilogConfiguration.FileSinkOptions>();
        if (fileSinkConfig != null)
        {
            loggerConfiguration
                .WriteTo.File(
                    path: fileSinkConfig.FilePath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: fileSinkConfig.RetainedFileCountLimit,
                    outputTemplate: fileSinkConfig.OutputTemplate)
                .Filter
                .ByExcluding(IsHealthCheckLogEvent);
        }

        return loggerConfiguration;
    }
    
    private static bool IsHealthCheckLogEvent(LogEvent logEvent)
    {
        return logEvent.Properties.Any(kvp => kvp.Value.ToString().Equals("\"/health\""));
    }
}