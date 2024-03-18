using KafkaFlow;

namespace Producer.Applications;

public class HelloMessageProducer
{
    private readonly IMessageProducer<HelloMessageProducer> _producer;

    public HelloMessageProducer(IMessageProducer<HelloMessageProducer> producer)
    {
        _producer = producer;
    }
    
}