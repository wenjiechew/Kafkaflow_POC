using SharedLibrary.Core.Abstractions;

namespace SharedLibrary.Core.Contracts;

public class HelloMessage : Entity<HelloId>
{
    public string? MessageId { get; set; }
    public string? Text { get; set; }

    public HelloMessage(HelloId id) 
        : base(id)
    {
        MessageId = id.Value.ToString();
    }
}