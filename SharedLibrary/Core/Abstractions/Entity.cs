namespace SharedLibrary.Core.Abstractions;

public abstract class Entity<TEntity>
{
    protected Entity(TEntity id)
    {
        Id = id;
    }
    public TEntity Id { get; set; }
}
