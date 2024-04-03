using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.Serializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedLibrary.KafkaFlow.Configurations;
using SharedLibrary.KafkaFlow.Configurations.Consumers;
using SharedLibrary.KafkaFlow.Configurations.Producers;
using SharedLibrary.KafkaFlow.Extensions;
using SharedLibrary.KafkaFlow.SharedMiddlewares;
using KafkaConfiguration = SharedLibrary.KafkaFlow.Configurations.KafkaConfiguration;

namespace SharedLibrary.KafkaFlow.DependencyInjections;

public static class KafkaFlowService
{
    private static IEnumerable<ConsumerSettings> GetConsumerSettings(KafkaConfiguration.KafkaSettings settings)
    {
        var consumerSettings = settings.Consumers;

        return consumerSettings;
    }


    public static IServiceCollection AddKafkaConsumer(this IServiceCollection services, IConfiguration configuration,
        Dictionary<TargetConsumer, ConsumerSetup> consumerSetups)
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

                        var consumerSettings = GetConsumerSettings(kafkaSettings);

                        foreach (var consumerSetting in consumerSettings)
                        {
                            cluster.AddConsumer(consumer =>
                            {
                                consumer
                                    .Topics(consumerSetting.Topics)
                                    .WithGroupId(consumerSetting.GroupId)
                                    .WithBufferSize(consumerSetting.BufferSize)
                                    .WithWorkersCount(consumerSetting.WorkersCount)
                                    .ConfigureDistributionStrategy(consumerSetting.DistributionStrategy);

                                var consumerSetup = consumerSetups.FirstOrDefault(x => x.Key.ConsumerNo == consumerSetting.ConsumerNo).Value;
                                
                                consumer.AddMiddlewares(middleware =>
                                {
                                    AddDefaultConsumerMiddlewares(middleware, consumerSetup!.ConfigureMiddlewares);

                                    middleware.AddTypedHandlers(typeHandlerBuilder =>
                                    {
                                        consumerSetup.ConfigureHandlers?.Invoke(typeHandlerBuilder);
                                    });
                                });
                            });
                        }
                    }));
        }

        return services;
    }

    // Encapsulate default middleware addition, including CorrelationIdLoggingMiddleware
    private static void AddDefaultConsumerMiddlewares(IConsumerMiddlewareConfigurationBuilder middleware,
        Action<IConsumerMiddlewareConfigurationBuilder>? configureMiddlewares)
    {
        middleware.Add<CorrelationIdLoggingKafkaFlowMiddleware>();
        middleware.Add<WorkerIdLoggingMiddleware>();
        
        configureMiddlewares?.Invoke(middleware);
        middleware.AddDeserializer<JsonCoreDeserializer>();
    }
    
    private static void AddDefaultProducerMiddleware(IProducerMiddlewareConfigurationBuilder middleware,
        Action<IProducerMiddlewareConfigurationBuilder>? configureMiddleware)
    {
        middleware.Add<CorrelationIdLoggingKafkaFlowMiddleware>();
        
        configureMiddleware?.Invoke(middleware);
        middleware.AddSerializer<JsonCoreSerializer>();
    }

    public static IServiceCollection AddKafkaProducer(this IServiceCollection services, IHostEnvironment environment, IConfiguration configuration,
        Dictionary<TargetProducer, ProducerSetup> producerSetups)
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

                        foreach (var producerSetting in kafkaSettings.Producers)
                        {
                            var serviceName = string.IsNullOrEmpty(producerSetting.ProducerName)
                                ? environment.ApplicationName
                                : environment.ApplicationName + $"_{producerSetting.ProducerName}";

                            if (producerSetting.AutoCreateTopic)
                            {
                                cluster.CreateTopicIfNotExists(producerSetting.Topic, producerSetting.Partitions,
                                    producerSetting.ReplicationFactor);
                            }
                            
                            var producerSetup = producerSetups.FirstOrDefault(x => x.Key.ProducerNo == producerSetting.ProducerNo).Value;

                            cluster.AddProducer(
                                serviceName,
                                producer =>
                                {
                                    producer
                                        .DefaultTopic(producerSetting.Topic)
                                        .AddMiddlewares(middleware =>
                                        {
                                            AddDefaultProducerMiddleware(middleware, producerSetup!.ConfigureMiddlewares);
                                            
                                        });
                                });
                        }
                    }));
        }
        return services;
    }
}