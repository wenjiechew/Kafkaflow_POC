using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.Serializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedLibrary.KafkaFlow.Configurations;
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

                        var consumerSettings = GetConsumerSettings(kafkaSettings);

                        foreach (var consumerConfig in consumerSettings)
                        {
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
                                    AddDefaultMiddlewares(middleware, configureMiddlewares);

                                    middleware.AddTypedHandlers(typeHandlerBuilder =>
                                    {
                                        configureHandlers?.Invoke(typeHandlerBuilder);
                                    });
                                });
                            });
                        }
                    }));
        }

        return services;
    }

    // Encapsulate default middleware addition, including CorrelationIdLoggingMiddleware
    private static void AddDefaultMiddlewares(IConsumerMiddlewareConfigurationBuilder middleware,
        Action<IConsumerMiddlewareConfigurationBuilder>? configureMiddlewares)
    {
        middleware.Add<CorrelationIdLoggingKafkaFlowMiddleware>();
        middleware.Add<WorkerIdLoggingMiddleware>();
        configureMiddlewares?.Invoke(middleware);
        middleware.AddDeserializer<JsonCoreDeserializer>();
    }

    public static IServiceCollection AddKafkaProducer(this IServiceCollection services, IHostEnvironment environment, IConfiguration configuration,
        Action<IProducerMiddlewareConfigurationBuilder>? configureMiddleware)
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

                            cluster.AddProducer(
                                serviceName,
                                producer =>
                                {
                                    producer
                                        .DefaultTopic(producerSetting.Topic)
                                        .AddMiddlewares(middleware =>
                                        {
                                            middleware.Add<CorrelationIdLoggingKafkaFlowMiddleware>();


                                            middleware.AddSerializer<JsonCoreSerializer>();
                                            configureMiddleware?.Invoke(middleware);
                                        });
                                });
                        }
                    }));
        }
        return services;
    }
}