using PM.Shared.Dtos;

namespace PM.Shared.Handle.Interfaces
{
    public interface IRepository<TData, TEntity> where TData : DbContext where TEntity : class
    {
        public Task<ServiceResult<IEnumerable<TEntity>>> GetAllAsync();
        public Task<ServiceResult<IEnumerable<TEntity>>> GetManyAsync(string key, string valueKey);
        public Task<ServiceResult<TEntity>> GetOneAsync(string key, string valueKey);
        public Task<ServiceResult<bool>> IsExistName(IEnumerable<TEntity> arr, string name);
        public Task AddAsync(TEntity entity);
        public Task UpdateAsync(TEntity entity);
        public Task PacthAsync(TEntity entity, Dictionary<string, object> keyValuePairs);
        public Task DeleteAsync(TEntity entity);
    }

}