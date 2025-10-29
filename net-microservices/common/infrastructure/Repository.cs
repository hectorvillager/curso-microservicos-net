namespace webapi.common.infrastructure;

public interface IAdd<in T>
{
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
}

public interface IGet<T, in ID>{
    Task<T?> GetAsync(ID id, CancellationToken cancellationToken = default);   
}

public interface IUpdate<T, in ID> 
{
    Task UpdateAsync(ID id,T entity, CancellationToken cancellationToken = default);
}

public interface IRemove<in ID> 
{
    Task RemoveAsync(ID id, CancellationToken cancellationToken = default);
}

/*
public static class RepositoryExtensions
{
    public static async Task<T> GetOrThrowAsync<T, ID>(
        this DbContext context,
        ID id,
        CancellationToken cancellationToken = default) where T : class
    {
        var entity = await context.Set<T>().FindAsync(new object[] { id! }, cancellationToken);
        return entity ?? throw new KeyNotFoundException($"Entity with ID '{id}' not found.");
    }
}
*/