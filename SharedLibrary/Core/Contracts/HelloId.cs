namespace SharedLibrary.Core.Contracts;

public record HelloId(Guid Value)
{
    public static HelloId New => new(Guid.NewGuid());
}