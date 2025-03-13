using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using PM.Domain;
using PM.Domain.Interfaces;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using System.Numerics;

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

        #region GetOneByKeyAndValue Method
        /// <summary>
        /// Retrieves a single entity based on a dynamic key and value.
        /// </summary>
        /// <param name="key">The property name to filter by.</param>
        /// <param name="value">The value of the property to match.</param>
        /// <returns>A <see cref="ServicesResult{T}"/> containing the entity if found, otherwise an error message.</returns>
        public async Task<ServicesResult<T>> GetOneByKeyAndValue(string key, TKey value)
        {
            if (string.IsNullOrEmpty(key) || value == null)
                return new ServicesResult<T>("Invalid key or value");

            try
            {
                // Ensure the provided key exists in the entity type
                var property = typeof(T).GetProperty(key);
                if (property == null)
                    return new ServicesResult<T>($"Property '{key}' not found in {typeof(T).Name}");

                // Query database for the first matching entity
                var response = await _dbSet
                    .AsNoTracking()
                    .Where(x => EF.Property<TKey>(x, key).Equals(value))
                    .FirstOrDefaultAsync();

                // Return appropriate response based on query result
                if (response == null)
                    return new ServicesResult<T>("No matching record found");

                return new ServicesResult<T>(response);
            }
            catch (Exception ex)
            {
                // Log the error and return a generic error response
                _logger.LogError(ex, "Error occurred while querying data in repository.");
                return new ServicesResult<T>("An error occurred while retrieving data.");
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
                return ServicesResult<T>.Failure("Value is null or empty.");

            // Check if all properties exist in type T
            foreach (var key in value.Keys)
            {
                if (typeof(T).GetProperty(key) == null)
                    return ServicesResult<T>.Failure($"Property '{key}' not found in {typeof(T).Name}.");
            }

            try
            {
                IQueryable<T> query = _dbSet.AsNoTracking();
                var parameter = Expression.Parameter(typeof(T), "x");
                Expression? combinedExpression = null;

                foreach (var condition in value)
                {
                    var property = Expression.Property(parameter, condition.Key);
                    var constant = Expression.Constant(condition.Value);
                    var equalsExpression = Expression.Equal(property, constant);

                    combinedExpression = combinedExpression == null
                        ? equalsExpression
                        : useAndOperator
                            ? Expression.AndAlso(combinedExpression, equalsExpression)
                            : Expression.OrElse(combinedExpression, equalsExpression);
                }

                if (combinedExpression != null)
                {
                    var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
                    query = query.Where(lambda);
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
        /// Retrieves multiple entities based on a dynamic key-value pair.
        /// </summary>
        /// <param name="key">The name of the property to filter by.</param>
        /// <param name="value">The value to match against the specified property.</param>
        /// <returns>A service result containing a list of entities or an error message.</returns>
        public async Task<ServicesResult<IEnumerable<T>>> GetManyByKeyAndValue(string key, TKey value)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(key))
                return ServicesResult<IEnumerable<T>>.Failure("Key cannot be null or empty.");

            if (value == null)
                return ServicesResult<IEnumerable<T>>.Failure("Value cannot be null.");

            try
            {
                // Ensure the property exists in the entity type
                var property = typeof(T).GetProperty(key);
                if (property == null)
                    return ServicesResult<IEnumerable<T>>.Failure($"Property '{key}' not found in {typeof(T).Name}.");

                // Execute the query
                var response = await _dbSet
                    .AsNoTracking()
                    .Where(x => EF.Property<TKey>(x, key).Equals(value))
                    .ToListAsync();

                // Always return Success, even if the list is empty
                return ServicesResult<IEnumerable<T>>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while querying data in repository.");
                return ServicesResult<IEnumerable<T>>.Failure("An error occurred while retrieving data.");
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
                return ServicesResult<IEnumerable<T>>.Success(Enumerable.Empty<T>()); // Trả về danh sách rỗng

            if (pageNumber < 1 || pageSize < 1)
                return ServicesResult<IEnumerable<T>>.Failure("Invalid page number or page size. Both must be greater than zero.");

            // Check if all properties exist in type T
            foreach (var key in value.Keys)
            {
                if (typeof(T).GetProperty(key) == null)
                    return ServicesResult<IEnumerable<T>>.Failure($"Property '{key}' not found in {typeof(T).Name}.");
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
                    query = query.Where(combinedExpression);
                }

                var results = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                return ServicesResult<IEnumerable<T>>.Success(results); // Trả về dữ liệu, kể cả nếu rỗng
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
        public async Task<ServicesResult<bool>> AddAsync(List<T> arr, T entity, CancellationToken cancellationToken = default)
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
        public async Task<ServicesResult<bool>> UpdateAsync(List<T> arr, T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                return ServicesResult<bool>.Failure("Entity is null.");

            try
            {
                var identityProperty = typeof(T).GetProperty("Id");
                if (identityProperty == null)
                    return ServicesResult<bool>.Failure("Identity property 'Id' not found.");

                var identityValue = identityProperty.GetValue(entity);

                // Truy vấn trực tiếp kiểm tra Id có tồn tại trong DB không
                bool isCheckIdentity = await _dbSet.AnyAsync(x => EF.Property<object>(x, "Id").Equals(identityValue), cancellationToken);
                if (!isCheckIdentity)
                    return ServicesResult<bool>.Failure("Entity does not exist.");

                if (arr.Count == 0)
                {
                    _context.Update(entity);
                }
                else
                {
                    var nameProperty = typeof(T).GetProperty("Name");
                    if (nameProperty == null)
                        return ServicesResult<bool>.Failure("Property 'Name' not found.");

                    var entityNameValue = nameProperty.GetValue(entity);

                    // Kiểm tra trùng tên bằng cách lọc trên DB thay vì duyệt danh sách
                    bool isCheckName = await _dbSet.AnyAsync(x => EF.Property<object>(x, "Name").Equals(entityNameValue) && !EF.Property<object>(x, "Id").Equals(identityValue), cancellationToken);
                    if (isCheckName)
                        return ServicesResult<bool>.Failure("Name already exists.");

                    _context.Update(entity);
                }

                // Không cần SaveChangesAsync vì Unit of Work sẽ đảm nhiệm
                return ServicesResult<bool>.Success(true);
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
        /// <param name="arr">Collection of existing entities (already filtered).</param>
        /// <param name="primaryKey">The primary key of the entity to update.</param>
        /// <param name="updateValue">Dictionary containing property names and their new values.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>Returns success if the update is applied, otherwise returns a failure message.</returns>
        public async Task<ServicesResult<bool>> PatchAsync(List<T> arr, TKey primaryKey, Dictionary<string, object> updateValue, CancellationToken cancellationToken = default)
        {
            if (updateValue == null || updateValue.Count == 0)
                return ServicesResult<bool>.Failure("Update values are null or empty.");

            if (primaryKey == null)
                return ServicesResult<bool>.Failure("Primary key is null.");

            try
            {
                // Tìm entity trong danh sách arr thay vì truy vấn lại DB
                var entity = arr.FirstOrDefault(x => EF.Property<TKey>(x, "Id").Equals(primaryKey));

                if (entity == null)
                    return ServicesResult<bool>.Failure("Entity not found.");

                // Kiểm tra trùng lặp 'Name' trong danh sách đã tiền xử lý
                if (updateValue.ContainsKey("Name"))
                {
                    var newName = updateValue["Name"];
                    bool isDuplicate = arr.Any(x => EF.Property<object>(x, "Name").Equals(newName) && !EF.Property<TKey>(x, "Id").Equals(primaryKey));
                    if (isDuplicate)
                        return ServicesResult<bool>.Failure("Name already exists.");
                }

                // Cập nhật giá trị vào entity
                foreach (var item in updateValue)
                {
                    var property = entity.GetType().GetProperty(item.Key);
                    if (property == null || !property.CanWrite)
                        return ServicesResult<bool>.Failure($"Property '{item.Key}' not found or is read-only.");

                    try
                    {
                        // Chuyển đổi kiểu dữ liệu an toàn
                        object convertedValue = ConvertToType(item.Value, property.PropertyType);
                        property.SetValue(entity, convertedValue);
                    }
                    catch (Exception)
                    {
                        return ServicesResult<bool>.Failure($"Failed to convert value for property '{item.Key}'.");
                    }
                }

                // Đánh dấu entity là đã chỉnh sửa để UnitOfWork xử lý commit
                _context.Entry(entity).State = EntityState.Modified;

                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while partially updating data in the repository.");
                return ServicesResult<bool>.Failure("An error occurred while updating data.");
            }
        }

        /// <summary>
        /// Chuyển đổi giá trị về kiểu dữ liệu phù hợp.
        /// </summary>
        private object ConvertToType(object value, Type targetType)
        {
            if (targetType == typeof(Guid))
                return Guid.Parse(value.ToString()!);

            if (targetType == typeof(DateTime))
                return DateTime.Parse(value.ToString()!);

            if (Nullable.GetUnderlyingType(targetType) != null)
                return Convert.ChangeType(value, Nullable.GetUnderlyingType(targetType)!);

            return Convert.ChangeType(value, targetType);
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
