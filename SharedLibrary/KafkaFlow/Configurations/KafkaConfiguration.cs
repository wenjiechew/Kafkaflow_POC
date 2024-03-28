namespace SharedLibrary.KafkaFlow.Configurations;

public class KafkaConfiguration
{
    public class KafkaSettings
    {
        public string Brokers { get; set; }
        public List<ConsumerSettings> Consumers { get; set; } = new();
        public List<ProducerSettings> Producers { get; set; } = new();
    }

}

