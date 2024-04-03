using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using SharedLibrary.Core;
using SharedLibrary.Core.Constants;

namespace SharedLibrary.Logging.Extensions;

public static class SerilogOptionsExtension
{
    public static void AddApplicationNameProperty(this LoggerConfiguration loggerConfiguration, 
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var environmentSetting = configuration.GetSection("EnvironmentSetup").Get<EnvironmentSetup>();

        loggerConfiguration.Enrich.WithProperty(GlobalConstants.ApplicationNameHeader,
            environmentSetting != null
                ? $"{environmentSetting.EnvZone}{environmentSetting.EnvTenant}{environmentSetting.EnvAppName}"
                : environment.ApplicationName);
    }
}