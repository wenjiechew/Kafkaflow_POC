namespace SharedLibrary.Core.Contracts.Hello;

public record HelloId(Guid Value)
{
    public static HelloId New => new(Guid.NewGuid());
}