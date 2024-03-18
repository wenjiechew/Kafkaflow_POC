namespace SharedLibrary.KafkaFlow.Configurations;

public class KafkaConfiguration
{
    public class KafkaSettings
    {
        public string Brokers { get; set; }
        public ConsumerSettings Consumer { get; set; } = new();
        public ProducerSettings Producer { get; set; } = new();
    }

}

