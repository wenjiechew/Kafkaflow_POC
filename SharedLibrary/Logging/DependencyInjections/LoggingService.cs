using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using SharedLibrary.SeriLogging.Configurations;

namespace SharedLibrary.SeriLogging.DependencyInjections;

public static class LoggingService
{
    public static void Configure(LoggerConfiguration loggingConfiguration, IHostEnvironment environment, IConfiguration config)
    {
        loggingConfiguration
            .Enrich.FromLogContext()
            .WriteTo.Console();
        
        
        if (environment.IsDevelopment())
        {
            var fileSinkConfig = config.GetSection("FileSinkOptions").Get<SerilogConfiguration.FileSinkOptions>();
            if (fileSinkConfig != null)
                loggingConfiguration
                    .WriteTo.File(
                        path: fileSinkConfig.FilePath,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: fileSinkConfig.RetainedFileCountLimit,
                        outputTemplate: fileSinkConfig.OutputTemplate);

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
    

    

}