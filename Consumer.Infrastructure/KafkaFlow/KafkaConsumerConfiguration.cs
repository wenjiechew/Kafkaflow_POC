namespace Consumer.Infrastructure.KafkaFlow;

public class KafkaConsumerConfiguration
{
    public class KafkaSettings
    {
        public string Brokers { get; set; }
        public KafkaConsumerSettings Consumer { get; set; } = new();
    }

    public class KafkaConsumerSettings
    {
        public string GroupId { get; set; }
        public List<string> Topics { get; set; } = [];
        public int WorkersCount { get; set; }
        public string DistributionStrategy { get; set; }
    }
}