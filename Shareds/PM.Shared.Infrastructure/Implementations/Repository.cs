using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using LinqKit;
using PM.Shared.Infrastructure.Interfaces;

namespace PM.Shared.Infrastructure.Implementations
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

        public async Task<IEnumerable<TEntity>> GetAllAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> GetManyAsync(string key, string valueKey, CancellationToken cancellationToken = default)
        {
            var property = typeof(TEntity).GetProperty(key);
            if (property == null)
            {
                throw new ArgumentException($"Property '{key}' not found.");
            }

            ExpressionStarter<TEntity> predicate = PredicateBuilder.New<TEntity>();
            predicate = predicate.And(x => EF.Property<string>(x, key) == valueKey);

            return await _dbSet
                .AsNoTracking()
                .AsExpandable()
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }

        public async Task<TEntity?> GetOneAsync(string key, string valueKey, CancellationToken cancellationToken = default)
        {
            var property = typeof(TEntity).GetProperty(key);
            if (property == null)
            {
                throw new ArgumentException($"Property '{key}' not found.");
            }

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(x => EF.Property<string>(x, key) == valueKey, cancellationToken);
        }

        public Task<bool> IsExistName(IEnumerable<TEntity> arr, string name)
        {
            var property = typeof(TEntity).GetProperty("Name");
            if (property == null)
            {
                throw new ArgumentException("Property 'Name' not found.");
            }

            bool isExist = arr.Any(x => EF.Property<string>(x, "Name") == name);
            return Task.FromResult(isExist);
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
                {
                    throw new ArgumentException($"Property '{pair.Key}' not found.");
                }

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

        public Task DeleteAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            _dbSet.RemoveRange(entities);
            return Task.CompletedTask;
        }
    }
}
