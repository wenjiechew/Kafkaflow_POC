namespace SharedLibrary.KafkaFlow.Configurations;

public class ProducerSettings
{
    public string Topic { get; set; }
    public int Partitions { get; set; }
    public short ReplicationFactor { get; set; }
}