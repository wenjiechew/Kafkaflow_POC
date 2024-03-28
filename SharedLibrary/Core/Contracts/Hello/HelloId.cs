namespace SharedLibrary.Core.Abstractions;

public record HelloId(Guid Value)
{
    public static HelloId New => new(Guid.NewGuid());
}