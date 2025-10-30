using webapi.common.domain;

namespace webapi.common.infrastructure;

public interface IAdd<in T>
{
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
}

public interface IGet<T, in ID>
{
    Task<T> GetAsync(ID id, CancellationToken cancellationToken = default);
}

public interface IUpdate<T, in ID> : IGet<T, ID>
{
    void UpdateAsync(T entity, CancellationToken cancellationToken = default);
}

public interface IRemove<T, in ID> : IGet<T, ID>
{
    void RemoveAsync(T entity, CancellationToken cancellationToken = default);
}

public interface IGetOrThrowAsync
{
    Task<T> GetOrThrowAsync<T, ID>(ID id,
        CancellationToken cancellationToken = default,bool tracking=true) where T : Entity;
}

