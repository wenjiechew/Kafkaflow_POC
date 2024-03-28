using Serilog;
using SharedLibrary;
using SharedLibrary.Core.Contracts;
using SharedLibrary.SeriLogging.DependencyInjections;

namespace Producer;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo
            .Console()
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting Producer...");
            await CreateHostBuilder(args).Build().RunAsync();
            Log.Information("Application started successfully");

            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application start-up failed");

            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog((hostContext, loggerConfiguration) =>
                loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration))
            //LoggingService.Configure(loggerConfiguration, hostContext.HostingEnvironment, hostContext.Configuration))
            .ConfigureWebHostDefaults(webBuilder =>
            {

                webBuilder.UseStartup<Startup>();
            })
            .ConfigureAppConfiguration((hostContext, config) =>
            {

            });

}
