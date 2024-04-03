using KafkaFlow.Configuration;

namespace SharedLibrary.KafkaFlow.Configurations.Producers;

public class ProducerSetup
{
    public Action<IProducerMiddlewareConfigurationBuilder> ConfigureMiddlewares { get; set; }
}