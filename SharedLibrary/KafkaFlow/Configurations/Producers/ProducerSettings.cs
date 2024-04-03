namespace SharedLibrary.KafkaFlow.Configurations.Producers;

public class ProducerSettings
{
    public string ProducerName { get; set; }
    public string Topic { get; set; }
    public int Partitions { get; set; }
    public short ReplicationFactor { get; set; }
    public bool AutoCreateTopic { get; set; } = false;
    
    public int ProducerNo { get; set; }

}