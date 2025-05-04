using Microsoft.EntityFrameworkCore;
using PM.Shared.Dtos;

namespace PM.Shared.Handle.Interfaces
{
    public interface IRepository<TData, TEntity>
        where TData : DbContext
        where TEntity : class
    {
        Task<ServiceResult<IEnumerable<TEntity>>> GetAllAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<TEntity>>> GetManyAsync(string key, string valueKey, CancellationToken cancellationToken = default);

        Task<ServiceResult<TEntity>> GetOneAsync(string key, string valueKey, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> IsExistName(IEnumerable<TEntity> arr, string name);

        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task PatchAsync(TEntity entity, Dictionary<string, object> keyValuePairs, CancellationToken cancellationToken = default);

        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
        //Task DeleteAsync(TEntity entity, Func<TData, TEntity, Task>? deleteDependencies = null, CancellationToken cancellationToken = default);
        Task DeleteAsync(List<TEntity> entities, CancellationToken cancellationToken = default);
    }
}
