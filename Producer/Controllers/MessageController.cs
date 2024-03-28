using KafkaFlow;
using KafkaFlow.Producers;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Core.Constants;
using SharedLibrary.Core.Contracts.Hello;
using System.Text;

namespace Producer.Controllers;


[Route("api/[controller]")]
[ApiController]
public class MessageController : ControllerBase
{
    private const string Topic = "topic-1";

    private readonly IProducerAccessor _producers;

    private string ProducerName { get; }

    public MessageController(IProducerAccessor producers, IHostEnvironment environment)
    {
        _producers = producers;

        ProducerName = $"{environment.ApplicationName}_{Topic}";
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] string message)
    {
        var correlationId = HttpContext.Items[GlobalConstants.CorrelationIdHeader]?.ToString();

        var headers = new MessageHeaders
        {
            { GlobalConstants.CorrelationIdHeader, Encoding.UTF8.GetBytes(correlationId!) }
        };

        var producer = _producers.GetProducer(ProducerName);
        await producer.ProduceAsync(Topic, Guid.NewGuid().ToString(), new HelloMessage(HelloId.New) { Text = message }, headers);

        return Ok();
    }
}
