using KafkaFlow;
using KafkaFlow.Consumers.DistributionStrategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.KafkaFlow.KafkaFlowSharedMiddlewares;

namespace Consumer.Infrastructure.KafkaFlow;

public static class KafkaConsumerService
{
    public static IServiceCollection AddKafkaConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaSettings = configuration.GetSection("KafkaSettings").Get<KafkaConsumerConfiguration.KafkaSettings>();

        if (kafkaSettings != null)
        {
            services.AddKafkaFlowHostedService(kafka =>
                kafka.AddCluster(cluster =>
                {
                    cluster.WithBrokers(new[] { kafkaSettings.Brokers });
                    cluster.AddConsumer(consumer =>
                    {
                        consumer
                            .Topics(kafkaSettings.Consumer.Topics)
                            .WithGroupId(kafkaSettings.Consumer.GroupId)
                            .WithWorkersCount(kafkaSettings.Consumer.WorkersCount);
                            
                            // Add the distribution strategy
                            switch (kafkaSettings.Consumer.DistributionStrategy)
                            {
                                case "ByteSum":
                                    //default is ByteSum
                                    break;
                                case "FreeWorker":
                                    consumer.WithWorkerDistributionStrategy<FreeWorkerDistributionStrategy>();
                                    break;
                            }

                            consumer.AddMiddlewares(middleware => middleware
                                .Add<CoherenceIIdLoggingMiddleware>());
                    });
                })); 
        }
                    
            
        return services;
    }
}