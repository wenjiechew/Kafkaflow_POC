namespace SharedLibrary.KafkaFlow.Configurations.Consumers;

public class ConsumerSettings
{
    public string GroupId { get; set; }
    public List<string> Topics { get; set; } = [];
    public int WorkersCount { get; set; }
    public string DistributionStrategy { get; set; }
    public int BufferSize { get; set; }
    
    public int ConsumerNo { get; set; }
}