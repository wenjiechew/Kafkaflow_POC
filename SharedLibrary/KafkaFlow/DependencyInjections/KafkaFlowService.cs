using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.Serializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedLibrary.KafkaFlow.Extensions;
using SharedLibrary.KafkaFlow.SharedMiddlewares;
using KafkaConfiguration = SharedLibrary.KafkaFlow.Configurations.KafkaConfiguration;

namespace SharedLibrary.KafkaFlow.DependencyInjections;

public static class KafkaFlowService
{
    public static IServiceCollection AddKafkaConsumer(this IServiceCollection services, IConfiguration configuration,
        Action<IConsumerMiddlewareConfigurationBuilder>? configureMiddlewares,
        Action<TypedHandlerConfigurationBuilder>? configureHandlers)
    {
        var kafkaSettings = configuration.GetSection("Kafka").Get<KafkaConfiguration.KafkaSettings>();

        if (kafkaSettings != null)
        {
            services.AddKafkaFlowHostedService(kafka =>
                kafka
                    .UseConsoleLog()
                    .UseMicrosoftLog()
                    .AddCluster(cluster =>
                {
                    cluster.WithBrokers(new[] { kafkaSettings.Brokers });

                    var consumerConfig = kafkaSettings.Consumer;

                    cluster.AddConsumer(consumer =>
                    {
                        consumer
                            .Topics(consumerConfig.Topics)
                            .WithGroupId(consumerConfig.GroupId)
                            .WithBufferSize(consumerConfig.BufferSize)
                            .WithWorkersCount(consumerConfig.WorkersCount)
                            .ConfigureDistributionStrategy(consumerConfig.DistributionStrategy);

                        consumer.AddMiddlewares(middleware =>
                        {
                            middleware.Add<CorrelationIIdLoggingMiddleware>();
                            configureMiddlewares?.Invoke(middleware);
                            middleware.AddDeserializer<JsonCoreDeserializer>();
                            
                            middleware.AddTypedHandlers(typeHandlerBuilder =>
                            {
                                configureHandlers?.Invoke(typeHandlerBuilder);
                            });
                        });
                    });
                }));
        }

        return services;
    }

    public static IServiceCollection AddKafkaProducer(this IServiceCollection services, IHostEnvironment environment, IConfiguration configuration,
        Action<IProducerMiddlewareConfigurationBuilder>? configureMiddleware)
    {
        var kafkaSettings = configuration.GetSection("Kafka").Get<KafkaConfiguration.KafkaSettings>();

        if (kafkaSettings != null)
        {
            services.AddKafkaFlowHostedService(kafka =>
                kafka
                    .UseMicrosoftLog()
                    .AddCluster(cluster =>
                {
                    if (environment.IsDevelopment())
                        cluster.CreateTopicIfNotExists(kafkaSettings.Producer.Topic, kafkaSettings.Producer.Partitions, kafkaSettings.Producer.ReplicationFactor);
                    
                    cluster.WithBrokers(new[] { kafkaSettings.Brokers });
                    
                    var serviceName = environment.ApplicationName;
                    
                    cluster.AddProducer(
                        serviceName,
                        producer =>
                    {
                        producer
                            .DefaultTopic(kafkaSettings.Producer.Topic)
                            .AddMiddlewares(middleware =>
                            {
                                //middleware.Add<CorrelationIIdLoggingMiddleware>();
                                middleware.AddSerializer<JsonCoreSerializer>();
                                
                                configureMiddleware?.Invoke(middleware);
                            });
                    }); 
                    
                    
                }));
        }

        return services;
    }
}