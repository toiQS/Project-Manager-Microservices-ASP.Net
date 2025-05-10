using Microsoft.EntityFrameworkCore;

namespace PM.Shared.Infrastructure.Interfaces
{
    public interface IRepository<TData, TEntity> where TData : DbContext
        where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetManyAsync(string key, string valueKey, CancellationToken cancellationToken = default);
        Task<TEntity?> GetOneAsync(string key, string valueKey, CancellationToken cancellationToken = default);
        Task<bool> IsExistName(IEnumerable<TEntity> arr, string name);
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task PatchAsync(TEntity entity, Dictionary<string, object> keyValuePairs, CancellationToken cancellationToken = default);
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(List<TEntity> entities, CancellationToken cancellationToken = default);
    }
}
