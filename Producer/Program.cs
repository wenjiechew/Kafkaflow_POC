using KafkaFlow.Producers;
using Microsoft.Extensions.Hosting.Internal;
using Producer.Applications;
using Serilog;
using SharedLibrary;
using SharedLibrary.Core.Abstractions;
using SharedLibrary.Core.Contracts;
using SharedLibrary.Core.Contracts.Hello;

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
            var builder = CreateHostBuilder(args).Build();
            Log.Information("Application started successfully");
            
            //Just testing purpose
            var serviceName = builder.Services.GetRequiredService<IHostEnvironment>().ApplicationName;
            var producer = builder.Services.GetRequiredService<IProducerAccessor>().GetProducer(serviceName);


            await producer.ProduceAsync("topic-1", Guid.NewGuid().ToString(), new HelloMessage(HelloId.New) { Text = "Hello World" });
            _ = builder.RunAsync();



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
