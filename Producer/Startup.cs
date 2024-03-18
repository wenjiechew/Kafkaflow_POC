using KafkaFlow;
using KafkaFlow.Serializer;
using Producer.Applications;
using SharedLibrary.KafkaFlow.DependencyInjections;
using SharedLibrary.KafkaFlow.SharedMiddlewares;

namespace Producer;

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

        // TODO 
        // what is the best way to name the producer?
        var serviceName = Environment.ApplicationName;

        services.AddKafkaProducer(Environment, Configuration,
            middlewareBuilder =>
            {
                middlewareBuilder.Add<CorrelationIIdLoggingMiddleware>();
            });
        

        
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
        });
    }
}