using Consumer.Applications;
using KafkaFlow;
using SharedLibrary.KafkaFlow.DependencyInjections;

namespace Consumer;
public class Startup
{
    public IConfiguration Configuration { get; }
    public IWebHostEnvironment Environment { get; }


    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;

    }
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddHealthChecks();

        services.AddKafkaConsumer(Configuration,
            middlewareBuilder =>
            {

            },
            typedHandlerBuilder =>
            {
                typedHandlerBuilder.AddHandler<HelloMessageHandler>();
            });

    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
    {
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
        });

        var kafkabus = app.ApplicationServices.CreateKafkaBus();
        lifetime.ApplicationStarted.Register(() => kafkabus.StartAsync(lifetime.ApplicationStopping));
    }


}
