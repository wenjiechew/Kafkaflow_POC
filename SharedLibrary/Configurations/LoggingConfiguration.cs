using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using SharedLibrary.Core.Entities.Logging;

namespace SharedLibrary.Configurations;

public static class LoggingConfiguration
{
    public static void Configure(LoggerConfiguration loggingConfiguration, IHostEnvironment environment, IConfiguration config)
    {
        loggingConfiguration.Enrich.FromLogContext();
        
        if (environment.IsDevelopment())
        {
            var fileSinkConfig = config.GetSection("FileSinkOptions").Get<SerilogOptions.FileSinkOptions>();
            if (fileSinkConfig != null)
                loggingConfiguration
                    .WriteTo.File(
                        path: fileSinkConfig.FilePath,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: fileSinkConfig.RetainedFileCountLimit,
                        outputTemplate: fileSinkConfig.OutputTemplate)
                    .Filter
                    .ByExcluding(IsHealthCheckLogEvent);
            else
            {
                loggingConfiguration
                    .WriteTo.Console();
            }
        }
        else
        {
            loggingConfiguration.WriteTo.Console();
        }
    }
    
    private static bool IsHealthCheckLogEvent(LogEvent logEvent)
    {
        return logEvent.Properties.Any(kvp => kvp.Value.ToString().Equals("\"/health\""));
    }
    

}