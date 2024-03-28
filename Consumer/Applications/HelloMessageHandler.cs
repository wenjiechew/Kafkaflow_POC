using KafkaFlow;
using SharedLibrary.Core.Contracts;
using SharedLibrary.SeriLogging.Extensions;

namespace Consumer.Applications;

public class HelloMessageHandler : IMessageHandler<HelloMessage>
{
    private const string ClassName = nameof(HelloMessageHandler);
    private readonly ILogger<HelloMessageHandler> _logger;

    public HelloMessageHandler(ILogger<HelloMessageHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(IMessageContext context, HelloMessage message)
    {
        _logger.LogInfo($"Partition: [{context.ConsumerContext.Partition}] | Offset: {context.ConsumerContext.Offset} | Message: {message.MessageId}{message.Text}");

        return Task.CompletedTask;
    }
}