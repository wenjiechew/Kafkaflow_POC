using KafkaFlow;
using SharedLibrary.Core.Contracts;
using SharedLibrary.Core.Contracts.Hello;
using SharedLibrary.Logging.Extensions;
using SharedLibrary.SeriLogging.Extensions;

namespace Consumer.Applications;

public class HelloMessageHandler: IMessageHandler<HelloMessage>
{
    private readonly ILogger<HelloMessageHandler> _logger;

    public HelloMessageHandler(ILogger<HelloMessageHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(IMessageContext context, HelloMessage message)
    {
        _logger.LogInfo($"Message: {message.MessageId} : {message.Text}");

        return Task.CompletedTask;
    }
}