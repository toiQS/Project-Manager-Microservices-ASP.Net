using Microsoft.EntityFrameworkCore;
using LinqKit;
using PM.Shared.Handle.Interfaces;
using PM.Shared.Dtos.auths;

namespace PM.Shared.Handle.Implements
{
    public class Repository<TData, TEntity> : IRepository<TData, TEntity>
        where TData : DbContext
        where TEntity : class
    {
        private readonly TData _context;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(TData context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task<ServiceResult<IEnumerable<TEntity>>> GetAllAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _dbSet
                    .AsNoTracking()
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                return ServiceResult<IEnumerable<TEntity>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<TEntity>>.FromException(ex);
            }
        }

        public async Task<ServiceResult<IEnumerable<TEntity>>> GetManyAsync(string key, string valueKey, CancellationToken cancellationToken = default)
        {
            try
            {
                if (typeof(TEntity).GetProperty(key) == null)
                    return ServiceResult<IEnumerable<TEntity>>.Error("Property not found");

                var predicate = PredicateBuilder.New<TEntity>();
                predicate = predicate.And(x => EF.Property<string>(x, key) == valueKey);

                var response = await _dbSet
                    .AsNoTracking()
                    .AsExpandable()
                    .Where(predicate)
                    .ToListAsync(cancellationToken);

                return ServiceResult<IEnumerable<TEntity>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<TEntity>>.FromException(ex);
            }
        }

        public async Task<ServiceResult<TEntity>> GetOneAsync(string key, string valueKey, CancellationToken cancellationToken = default)
        {
            try
            {
                if (typeof(TEntity).GetProperty(key) == null)
                    return ServiceResult<TEntity>.Error("Property not found");

                var response = await _dbSet
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => EF.Property<string>(x, key) == valueKey, cancellationToken);

                return response == null
                    ? ServiceResult<TEntity>.Error("Data not found")
                    : ServiceResult<TEntity>.Success(response);
            }
            catch (Exception ex)
            {
                return ServiceResult<TEntity>.FromException(ex);
            }
        }

        public Task<ServiceResult<bool>> IsExistName(IEnumerable<TEntity> arr, string name)
        {
            if (typeof(TEntity).GetProperty("Name") == null)
                return Task.FromResult(ServiceResult<bool>.Error("Property not found"));

            var isExist = arr.Any(x => EF.Property<string>(x, "Name") == name);

            return isExist
                ? Task.FromResult(ServiceResult<bool>.Error("Name already exists"))
                : Task.FromResult(ServiceResult<bool>.Success(true));
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public Task PatchAsync(TEntity entity, Dictionary<string, object> keyValuePairs, CancellationToken cancellationToken = default)
        {
            foreach (var pair in keyValuePairs)
            {
                var property = typeof(TEntity).GetProperty(pair.Key);
                if (property == null)
                    throw new ArgumentException($"Property '{pair.Key}' not found");

                property.SetValue(entity, pair.Value);
            }

            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

    }
}
