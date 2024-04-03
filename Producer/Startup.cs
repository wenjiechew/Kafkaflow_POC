using Microsoft.OpenApi.Models;
using SharedLibrary.KafkaFlow.Configurations.Producers;
using SharedLibrary.KafkaFlow.DependencyInjections;
using SharedLibrary.Middlewares;

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

        var setups = new Dictionary<TargetProducer, ProducerSetup>
        {
            {
                new TargetProducer(1), new ProducerSetup
                {
                    ConfigureMiddlewares = middlewareBuilder => { }

                }
            }
        };
        services.AddKafkaProducer(Environment, Configuration, setups);

        // Register the Swagger generator, defining one or more Swagger documents
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

        });


    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Interface v1"));
        }

        app.UseMiddleware<CorrelationIdLoggingApiMiddleware>();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
        });
    }
}