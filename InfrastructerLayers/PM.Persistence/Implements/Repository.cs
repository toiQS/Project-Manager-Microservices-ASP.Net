using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PM.Domain;
using PM.Domain.Interfaces;
using System.Linq.Expressions;

namespace PM.Persistence.Implements
{
    public class Repository<T, TKey> : IRepository<T, TKey> where T : class where TKey : notnull
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly ILogger<Repository<T, TKey>> _logger;

        public Repository(ApplicationDbContext context, ILogger<Repository<T, TKey>> logger)
        {
            _context = context;
            _logger = logger;
            _dbSet = _context.Set<T>();

        }
        // Truy vấn dữ liệu (Query Methods)
        #region Data Retrieval - GetAllAsync
        /// <summary>
        /// Retrieves a paginated list of entities from the database.
        /// </summary>
        /// <param name="pageNumber">The current page number (must be >= 1).</param>
        /// <param name="pageSize">The number of items per page (must be >= 1).</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A service result containing the paginated list of entities or an error message.</returns>
        public async Task<ServicesResult<IEnumerable<T>>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            // Validate input parameters
            if (pageNumber < 1 || pageSize < 1)
            {
                return new ServicesResult<IEnumerable<T>>("Invalid page number or page size. Both must be greater than zero.");
            }

            try
            {
                // Retrieve paginated data from the database
                var response = await _dbSet.AsNoTracking()
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                return new ServicesResult<IEnumerable<T>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while querying data in repository.");
                return new ServicesResult<IEnumerable<T>>("An error occurred while retrieving data.");
            }
        }
        #endregion

        #region Data Retrieval - GetOneByKeyAndValue
        /// <summary>
        /// Retrieves a single entity based on dynamic key-value conditions.
        /// </summary>
        /// <param name="value">Dictionary containing column names as keys and search values.</param>
        /// <param name="useAndOperator">Indicates whether conditions should be combined using AND or OR.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A service result containing the entity or an error message.</returns>
        public async Task<ServicesResult<T>> GetOneByKeyAndValue(Dictionary<string, TKey> value, bool useAndOperator, CancellationToken cancellationToken = default)
        {
            // Validate input parameters
            if (value == null || value.Count == 0)
            {
                return ServicesResult<T>.Failure("Value is null or empty.");
            }

            try
            {
                IQueryable<T> query = _dbSet;
                Expression<Func<T, bool>> combinedExpression = null!;
                var parameter = Expression.Parameter(typeof(T), "x");

                foreach (var condition in value)
                {
                    var property = Expression.Property(parameter, condition.Key);
                    var constant = Expression.Constant(condition.Value);
                    var equalsExpression = Expression.Equal(property, constant);
                    var lambda = Expression.Lambda<Func<T, bool>>(equalsExpression, parameter);

                    combinedExpression = combinedExpression == null
                        ? lambda
                        : useAndOperator
                            ? Expression.Lambda<Func<T, bool>>(Expression.AndAlso(combinedExpression.Body, lambda.Body), parameter)
                            : Expression.Lambda<Func<T, bool>>(Expression.OrElse(combinedExpression.Body, lambda.Body), parameter);
                }

                if (combinedExpression != null)
                {
                    query = query.Where(combinedExpression).AsNoTracking();
                }

                var result = await query.FirstOrDefaultAsync(cancellationToken);
                return result != null
                    ? ServicesResult<T>.Success(result)
                    : ServicesResult<T>.Failure("No matching record found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while querying data in repository.");
                return ServicesResult<T>.Failure("An error occurred while retrieving data.");
            }
        }
        #endregion

        #region Data Retrieval - GetManyByKeyAndValue
        /// <summary>
        /// Retrieves multiple entities based on dynamic key-value conditions with pagination.
        /// </summary>
        /// <param name="value">Dictionary containing column names as keys and search values.</param>
        /// <param name="useAndOperator">Indicates whether conditions should be combined using AND or OR.</param>
        /// <param name="pageNumber">Current page number for pagination.</param>
        /// <param name="pageSize">Number of records per page.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A service result containing a list of entities or an error message.</returns>
        public async Task<ServicesResult<IEnumerable<T>>> GetManyByKeyAndValue(Dictionary<string, TKey> value, bool useAndOperator, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            // Validate input parameters
            if (value == null || value.Count == 0)
                return ServicesResult<IEnumerable<T>>.Failure("Value is null or empty.");

            if (pageNumber < 1 || pageSize < 1)
                return ServicesResult<IEnumerable<T>>.Failure("Invalid page number or page size. Both must be greater than zero.");

            try
            {
                IQueryable<T> query = _dbSet;
                Expression<Func<T, bool>> combinedExpression = null!;
                var parameter = Expression.Parameter(typeof(T), "x");

                foreach (var condition in value)
                {
                    var property = Expression.Property(parameter, condition.Key);
                    var constant = Expression.Constant(condition.Value);
                    var equalsExpression = Expression.Equal(property, constant);
                    var lambda = Expression.Lambda<Func<T, bool>>(equalsExpression, parameter);

                    combinedExpression = combinedExpression == null
                        ? lambda
                        : useAndOperator
                            ? Expression.Lambda<Func<T, bool>>(Expression.AndAlso(combinedExpression.Body, lambda.Body), parameter)
                            : Expression.Lambda<Func<T, bool>>(Expression.OrElse(combinedExpression.Body, lambda.Body), parameter);
                }

                if (combinedExpression != null)
                {
                    query = query.Where(combinedExpression);
                }

                var results = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

                return results.Any()
                    ? ServicesResult<IEnumerable<T>>.Success(results)
                    : ServicesResult<IEnumerable<T>>.Failure("No matching records found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while querying data in repository.");
                return ServicesResult<IEnumerable<T>>.Failure("An error occurred while retrieving data.");
            }
        }
        #endregion
        // Thêm, cập nhật, xóa dữ liệu (Mutation Methods)
        #region Data Modification - AddAsync
        /// <summary>
        /// Adds a new entity to the database if it does not already exist in the provided list.
        /// </summary>
        /// <param name="arr">Collection of existing entities.</param>
        /// <param name="entity">The entity to be added.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>Returns success if the entity is added, otherwise returns a failure message.</returns>
        public async Task<ServicesResult<bool>> AddAsync(IEnumerable<T> arr, T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                return ServicesResult<bool>.Failure("Entity is null.");
            }
            try
            {
                if (!arr.Any())
                {
                    await _dbSet.AddAsync(entity, cancellationToken);

                    return ServicesResult<bool>.Success(true);
                }
                else
                {
                    var isCheckName = arr.Any(x => EF.Property<TKey>(x, "Name").Equals(EF.Property<TKey>(entity, "Name")));
                    if (isCheckName) return ServicesResult<bool>.Failure("Name already exists.");

                    await _dbSet.AddAsync(entity, cancellationToken);

                    return ServicesResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding data to the repository.");
                return ServicesResult<bool>.Failure("An error occurred while adding data.");
            }
        }
        #endregion

        #region Data Modification - UpdateAsync
        /// <summary>
        /// Updates an existing entity in the database if it does not conflict with existing records.
        /// </summary>
        /// <param name="arr">Collection of existing entities.</param>
        /// <param name="entity">The entity to be updated.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>Returns success if the entity is updated, otherwise returns a failure message.</returns>
        public async Task<ServicesResult<bool>> UpdateAsync(IEnumerable<T> arr, T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                return ServicesResult<bool>.Failure("Entity is null.");
            }
            try
            {
                if (!arr.Any())
                {
                    _dbSet.Update(entity);

                    return ServicesResult<bool>.Success(true);
                }
                else
                {
                    var isCheckName = arr.Any(x => EF.Property<TKey>(x, "Name").Equals(EF.Property<TKey>(entity, "Name")));
                    if (isCheckName) return ServicesResult<bool>.Failure("Name already exists.");

                    _context.Update(entity);

                    return ServicesResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating data in the repository.");
                return ServicesResult<bool>.Failure("An error occurred while updating data.");
            }
        }
        #endregion

        #region Data Modification - PatchAsync
        /// <summary>
        /// Partially updates an entity based on the provided key-value pairs.
        /// </summary>
        /// <param name="arr">Collection of existing entities.</param>
        /// <param name="primaryKey">The primary key of the entity to update.</param>
        /// <param name="updateValue">Dictionary containing property names and their new values.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>Returns success if the update is applied, otherwise returns a failure message.</returns>
        public async Task<ServicesResult<bool>> PatchAsync(IEnumerable<T> arr, TKey primaryKey, Dictionary<string, object> updateValue, CancellationToken cancellationToken = default)
        {
            if (updateValue == null || updateValue.Count == 0)
            {
                return ServicesResult<bool>.Failure("Update values are null or empty.");
            }
            if (primaryKey == null)
            {
                return ServicesResult<bool>.Failure("Primary key is null.");
            }
            try
            {
                // Truy vấn trực tiếp từ database để tránh làm việc với danh sách trên bộ nhớ
                var entity = await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => EF.Property<TKey>(x, "Id").Equals(primaryKey), cancellationToken);

                if (entity == null)
                {
                    return ServicesResult<bool>.Failure("Entity not found.");
                }

                foreach (var item in updateValue)
                {
                    var property = entity.GetType().GetProperty(item.Key);
                    if (property == null || !property.CanWrite)
                    {
                        return ServicesResult<bool>.Failure($"Property '{item.Key}' not found or is read-only.");
                    }

                    try
                    {
                        // Chuyển đổi kiểu dữ liệu trước khi gán giá trị
                        object convertedValue = Convert.ChangeType(item.Value, property.PropertyType);
                        property.SetValue(entity, convertedValue);
                    }
                    catch (Exception)
                    {
                        return ServicesResult<bool>.Failure($"Failed to convert value for property '{item.Key}'.");
                    }
                }

                // Cập nhật lại entity vào database
                _context.Update(entity);


                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while partially updating data in the repository.");
                return ServicesResult<bool>.Failure("An error occurred while updating data.");
            }
        }
        #endregion

        #region Data Modification - DeleteAsync
        /// <summary>
        /// Deletes an entity from the database based on its primary key.
        /// </summary>
        /// <param name="primaryKey">The primary key of the entity to delete.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>Returns success if the entity is deleted, otherwise returns a failure message.</returns>
        public async Task<ServicesResult<bool>> DeleteAsync(TKey primaryKey, CancellationToken cancellationToken = default)
        {
            if (primaryKey == null)
            {
                return ServicesResult<bool>.Failure("Primary key is null.");
            }
            try
            {
                var entity = await _dbSet.FindAsync(new object[] { primaryKey }, cancellationToken);
                if (entity == null)
                {
                    _logger.LogWarning("Delete failed: Entity with key {PrimaryKey} not found.", primaryKey);
                    return ServicesResult<bool>.Failure("Entity not found.");
                }

                _dbSet.Remove(entity);

                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting entity with key {PrimaryKey} in the repository.", primaryKey);
                return ServicesResult<bool>.Failure("An error occurred while deleting data.");
            }
        }
        #endregion

        #region Data Modification - DeleteManyAsync
        /// <summary>
        /// Deletes multiple entities based on dynamic key-value conditions.
        /// </summary>
        /// <param name="value">Dictionary containing column names as keys and search values.</param>
        /// <param name="useAndOperator">Indicates whether conditions should be combined using AND or OR.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>Returns success if records are deleted, otherwise returns a failure message.</returns>
        public async Task<ServicesResult<bool>> DeleteManyAsync(Dictionary<string, TKey> value, bool useAndOperator, CancellationToken cancellationToken = default)
        {
            if (value == null || value.Count == 0)
            {
                return ServicesResult<bool>.Failure("Value is null or empty.");
            }

            try
            {
                IQueryable<T> query = _dbSet.AsNoTracking();
                Expression<Func<T, bool>>? combinedExpression = null;
                var parameter = Expression.Parameter(typeof(T), "x");

                foreach (var condition in value)
                {
                    var property = typeof(T).GetProperty(condition.Key);
                    if (property == null)
                    {
                        _logger.LogWarning("Property '{PropertyName}' not found in entity '{EntityName}'.", condition.Key, typeof(T).Name);
                        return ServicesResult<bool>.Failure($"Property '{condition.Key}' not found.");
                    }

                    var propertyExpression = Expression.Property(parameter, condition.Key);
                    var constant = Expression.Constant(condition.Value);
                    var equalsExpression = Expression.Equal(propertyExpression, constant);
                    var lambda = Expression.Lambda<Func<T, bool>>(equalsExpression, parameter);

                    combinedExpression = combinedExpression == null
                        ? lambda
                        : useAndOperator
                            ? Expression.Lambda<Func<T, bool>>(Expression.AndAlso(combinedExpression.Body, lambda.Body), parameter)
                            : Expression.Lambda<Func<T, bool>>(Expression.OrElse(combinedExpression.Body, lambda.Body), parameter);
                }

                if (combinedExpression != null)
                {
                    query = query.Where(combinedExpression);
                }

                var entitiesToDelete = await query.ToListAsync(cancellationToken);
                if (entitiesToDelete.Count == 0)
                {
                    return ServicesResult<bool>.Failure("No matching records found.");
                }

                _dbSet.RemoveRange(entitiesToDelete);

                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting multiple entities in the repository.");
                return ServicesResult<bool>.Failure("An error occurred while deleting data.");
            }
        }
        #endregion



    }
}
