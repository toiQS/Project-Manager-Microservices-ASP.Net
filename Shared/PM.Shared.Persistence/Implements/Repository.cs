using Microsoft.EntityFrameworkCore;
using PM.Shared.Dtos;
using PM.Shared.Persistence.Interfaces;

namespace PM.Shared.Persistence.Implements
{
    public class Repository<TData, TEntity> : IRepository<TData, TEntity> where TData : DbContext where TEntity : class
    {
        private readonly TData _context;
        private readonly DbSet<TEntity> _dbSet;
        public Repository(TData context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task<ServiceResult<IEnumerable<TEntity>>> GetAllAsync()
        {
            try
            {
                CancellationToken cancellationToken = default;
                var response = await _dbSet.AsNoTracking().Skip(50 * 1).Take(50).ToListAsync(cancellationToken);
                return ServiceResult<IEnumerable<TEntity>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<TEntity>>.Failure(ex);
            }
        }
        public async Task<ServiceResult<IEnumerable<TEntity>>> GetManyAsync(string key, string valueKey)
        {
            try
            {
                bool isCheckProperty = typeof(TEntity).GetProperty(key) != null;
                if (!isCheckProperty)
                {
                   return ServiceResult<IEnumerable<TEntity>>.Failure("Property not found");
                }
                var response = await _dbSet.AsNoTracking().Where(x => EF.Property<string>(x, key) == valueKey).ToListAsync();
                return ServiceResult<IEnumerable<TEntity>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<TEntity>>.Failure(ex);
            }
        }
        public async Task<ServiceResult<TEntity>> GetOneAsync(string key, string valueKey)
        {
            try
            {
                bool isCheckProperty = typeof(TEntity).GetProperty(key) != null;
                if (!isCheckProperty)
                {
                    return ServiceResult<TEntity>.Failure("Property not found");
                }
                var response = await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => EF.Property<string>(x, key) == valueKey);
                if(response == null) return ServiceResult<TEntity>.Failure("Data not found");
                return ServiceResult<TEntity>.Success(response);
            }
            catch (Exception ex)
            {
                return ServiceResult<TEntity>.Failure(ex);
            }
        }
        public Task<ServiceResult<bool>> IsExistName(IEnumerable<TEntity> arr, string name)
        {
            var isCheckNameProperty = typeof(TEntity).GetProperty("Name") != null;
            if (!isCheckNameProperty)
            {
                return Task.FromResult(ServiceResult<bool>.Failure("Property not found"));
            }
            var isExist = arr.Any(x => EF.Property<string>(x, "Name") == name);
            if (isExist)
            {
                return Task.FromResult(ServiceResult<bool>.Failure("Name already exists"));
            }
            return Task.FromResult(ServiceResult<bool>.Success(true));
        }
        public Task AddAsync(TEntity entity)
        {
             _dbSet.AddAsync(entity);
            return Task.CompletedTask;
        }
        public Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }
        public Task PacthAsync(TEntity entity,Dictionary<string, object> keyValuePairs)
        {
            foreach (var keyValuePair in keyValuePairs)
            {
                var property = typeof(TEntity).GetProperty(keyValuePair.Key);
                if (property != null)
                {
                    property.SetValue(entity, keyValuePair.Value);
                }
                Task.FromException(new Exception("Property not found"));
            }
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }
        public Task DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }
    }
}
