using KafkaFlow.Configuration;
using KafkaFlow.Consumers.DistributionStrategies;

namespace SharedLibrary.KafkaFlow.Extensions;

public static class KafkaExtension
{
    public static IConsumerConfigurationBuilder ConfigureDistributionStrategy(this IConsumerConfigurationBuilder builder, string distributionStrategyName)
    {
        // Match the distribution strategy name to the corresponding KafkaFlow strategy
        switch (distributionStrategyName)
        {
            case "FreeWorker":
                builder.WithWorkerDistributionStrategy<FreeWorkerDistributionStrategy>();
                break;
        }

        return builder;
    }
}