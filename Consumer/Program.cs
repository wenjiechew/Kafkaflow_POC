using KafkaFlow;
using Serilog;

namespace Consumer;

public static class Program
{
    public static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo
            .Console()
            .CreateBootstrapLogger();
        
        try
        {
            Log.Information("Starting Consumer...");
            CreateHostBuilder(args).Build().Run();

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
            
            Log.CloseAndFlush();
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
