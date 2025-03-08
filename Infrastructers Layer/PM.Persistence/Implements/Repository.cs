using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Identity.Client;
using PM.Domain;
using PM.Domain.Interfaces;

namespace PM.Persistence.Implements
{
    public class Repository<T, TKey> : IRepository<T, TKey> where T : class where TKey : notnull
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<ServicesResult<IEnumerable<T>>> GetAllAsync()
        {
            try
            {
                var response = await _dbSet.AsNoTracking().AsSplitQuery().ToListAsync();
                return ServicesResult<IEnumerable<T>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<T>>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServicesResult<T>> AddAsync(T entity)
        {
            if (entity is null) return ServicesResult<T>.Failure("Entity is required");

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _dbSet.AddAsync(entity);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return ServicesResult<T>.Success(entity);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return ServicesResult<T>.Failure($"Database access error: {ex.Message}");
                }
            }
        }

        public async Task<ServicesResult<T>> UpdateAsync(T entity)
        {
            if (entity is null) return ServicesResult<T>.Failure("Entity is required");

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _dbSet.Update(entity);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return ServicesResult<T>.Success(entity);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return ServicesResult<T>.Failure($"Database access error: {ex.Message}");
                }
            }
        }

        public async Task<ServicesResult<bool>> DeleteAsync(TKey primaryKey)
        {
            if (primaryKey == null)
                return ServicesResult<bool>.Failure("Primary key cannot be null.");

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var entity = await _dbSet.FirstOrDefaultAsync(t =>
                        EF.Property<object>(t, "Id").Equals(primaryKey));

                    if (entity == null)
                    {
                        return ServicesResult<bool>.Failure("Entity not found.");
                    }

                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return ServicesResult<bool>.Success(true);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return ServicesResult<bool>.Failure($"Database access error: {ex.Message}");
                }
            }
        }

        public async Task<bool> ExistsAsync(TKey primaryKey)
        {
            if (primaryKey == null) return false;
            return await _dbSet.AnyAsync(t => EF.Property<object>(t, "Id").Equals(primaryKey));
        }
        public async Task<bool> ExistAsync(string key, TKey value)
        {
            if (string.IsNullOrEmpty(key) || value == null)
                return false;
            return await _dbSet.AnyAsync(t => EF.Property<object>(t, key).Equals(value));
        }


        public async Task<ServicesResult<T>> GetOneByKeyAndValue(string key, TKey value)
        {
            if (string.IsNullOrWhiteSpace(key) || value == null)
                return ServicesResult<T>.Failure("Key or value cannot be null.");

            try
            {
                var entity = await _dbSet.FirstOrDefaultAsync(t =>
                    EF.Property<object>(t, key).Equals(value));

                return entity != null ? ServicesResult<T>.Success(entity) : ServicesResult<T>.Failure("Entity not found.");
            }
            catch (Exception ex)
            {
                return ServicesResult<T>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServicesResult<IEnumerable<T>>> GetManyByKeyAndValue(string key, TKey value)
        {
            if (string.IsNullOrWhiteSpace(key) || value == null)
                return ServicesResult<IEnumerable<T>>.Failure("Key or value cannot be null.");

            try
            {
                var entities = await _dbSet.Where(t =>
                    EF.Property<object>(t, key).Equals(value)).ToListAsync();

                return ServicesResult<IEnumerable<T>>.Success(entities);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<T>>.Failure($"Error: {ex.Message}");
            }
        }


        public async Task<ServicesResult<(IEnumerable<T> Items, int TotalCount)>> GetPagedAsync(int pageNumber, int pageSize)
        {
            try
            {
                var totalItems = await _dbSet.CountAsync();
                var items = await _dbSet.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
                return ServicesResult<(IEnumerable<T> Items, int TotalCount)>.Success((items, totalItems));
            }
            catch (Exception ex)
            {
                return ServicesResult<(IEnumerable<T> Items, int TotalCount)>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServicesResult<bool>> SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Failure($"Error: {ex.Message}");
            }
        }



        /// <summary>
        /// update logic of action add
        /// </summary>
        public async Task<ServicesResult<bool>> AddAsync(IEnumerable<T> arr, T entity)
        {
            if (entity is null) return ServicesResult<bool>.Failure("Entity is required");
            try
            {
                var findNameExist = arr.Any(t => EF.Property<object>(t, "Name").Equals(EF.Property<object>(entity, "Name")));
                if (findNameExist)
                {
                    return ServicesResult<bool>.Failure("Name is exist");
                }
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    await _dbSet.AddAsync(entity);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return ServicesResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Failure($"Database access error: {ex.Message}");
            }
        }
        /// <summary>
        /// update logic of action update
        /// </summary>

        public async Task<ServicesResult<bool>> UpdateAsync(IEnumerable<T> arr, T entity)
        {
            if (entity is null) return ServicesResult<bool>.Failure("Entity is required");
            try
            {
                var findNameExist = arr.Any(t => EF.Property<object>(t, "Name").Equals(EF.Property<object>(entity, "Name")));
                if (findNameExist)
                {
                    return ServicesResult<bool>.Failure("Name is exist");
                }
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    _dbSet.Update(entity);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return ServicesResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Failure($"Database access error: {ex.Message}");
            }
        }
        /// <summary>
        /// delele many entity
        /// </summary>
        public async Task<ServicesResult<bool>> DeleteAsync(string key, TKey value)
        {
            if (string.IsNullOrEmpty(key) || value == null)
                return ServicesResult<bool>.Failure("Key or value cannot be null.");
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    var arr = await _dbSet.Where(t =>
                        EF.Property<object>(t, key).Equals(value)).ToListAsync();
                    if (arr.Count == 0)
                    {
                        
                        return ServicesResult<bool>.Failure("Entity not found.");
                    }
                    foreach (var item in arr)
                    {
                        _dbSet.Remove(item);
                    }
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return ServicesResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Failure($"Database access error: {ex.Message}");
            }

        }

    }
}
