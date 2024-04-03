using Consumer.Applications;
using KafkaFlow;
using SharedLibrary.KafkaFlow.Configurations;
using SharedLibrary.KafkaFlow.Configurations.Consumers;
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

        var setups = new Dictionary<TargetConsumer, ConsumerSetup>
        {
            {new TargetConsumer(1), new ConsumerSetup
            {
                ConfigureMiddlewares = middlewareBuilder => { },
                ConfigureHandlers = typedHandlerBuilder =>
                {
                    typedHandlerBuilder.AddHandler<HelloMessageHandler>();
                }
            }}
        };
        
        services.AddKafkaConsumer(Configuration, setups);

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

        var kafkaBus = app.ApplicationServices.CreateKafkaBus();
        lifetime.ApplicationStarted.Register(() => kafkaBus.StartAsync(lifetime.ApplicationStopping));
    }


}
