using KafkaFlow;
using KafkaFlow.Configuration;

namespace SharedLibrary.KafkaFlow.Configurations.Consumers;

public class ConsumerSetup
{
    public Action<IConsumerMiddlewareConfigurationBuilder> ConfigureMiddlewares { get; set; }
    public Action<TypedHandlerConfigurationBuilder> ConfigureHandlers { get; set; }
}