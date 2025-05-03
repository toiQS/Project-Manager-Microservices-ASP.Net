using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using System.Linq.Expressions;

namespace PM.Infrastructer.Implements
{
    public class QueryRepository<T, TKey> : IQueryRepository<T, TKey> where T : class where TKey : notnull
    {
        private readonly AuthDbContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly ILogger<QueryRepository<T, TKey>> _logger;
        

        public QueryRepository(AuthDbContext context, ILogger<QueryRepository<T, TKey>> logger)
        {
            _context = context;
            _logger = logger;
            _dbSet = _context.Set<T>();
            
        }
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
            if (pageNumber < 1 || pageSize < 1)
            {
                _logger.LogWarning("[Repository] GetAllAsync - Invalid pagination parameters: pageNumber={PageNumber}, pageSize={PageSize}", pageNumber, pageSize);
                return ServicesResult<IEnumerable<T>>.Failure("Invalid page number or page size. Both must be greater than zero.");
            }

            try
            {
                _logger.LogInformation("[Repository] GetAllAsync - Fetching data for page {PageNumber} with size {PageSize}", pageNumber, pageSize);

                var query = _dbSet.AsNoTracking();

                // Ensure consistent ordering to avoid random pagination issues
                if (_dbSet.EntityType.ClrType.GetProperty("Id") != null)
                {
                    query = query.OrderBy(e => EF.Property<object>(e, "Id"));
                }

                var response = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("[Repository] GetAllAsync - Retrieved {Count} records for page {PageNumber}", response.Count, pageNumber);

                return ServicesResult<IEnumerable<T>>.Success(response.Any() ? response : Enumerable.Empty<T>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Repository] GetAllAsync - Error occurred while querying data for page {PageNumber} with size {PageSize}", pageNumber, pageSize);
                return ServicesResult<IEnumerable<T>>.Failure("An error occurred while retrieving data.");
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
            {
                _logger.LogWarning("[Repository] GetOneByKeyAndValue - Invalid parameters: key={Key}, value={Value}", key, value);
                return ServicesResult<T>.Failure("Invalid key or value.");
            }

            try
            {
                _logger.LogInformation("[Repository] GetOneByKeyAndValue - Searching for entity where {Key}={Value} in {EntityName}", key, value, typeof(T).Name);

                // Ensure the provided key exists in the entity type
                var property = typeof(T).GetProperty(key);
                if (property == null)
                {
                    _logger.LogWarning("[Repository] GetOneByKeyAndValue - Property '{Key}' not found in {EntityName}", key, typeof(T).Name);
                    return ServicesResult<T>.Failure($"Property '{key}' not found in {typeof(T).Name}.");
                }

                // Query database for the first matching entity
                var response = await _dbSet
                    .AsNoTracking()
                    .Where(x => EF.Property<TKey>(x, key).Equals(value))
                    .FirstOrDefaultAsync();

                if (response == null)
                {
                    _logger.LogInformation("[Repository] GetOneByKeyAndValue - No record found where {Key}={Value} in {EntityName}", key, value, typeof(T).Name);
                    return ServicesResult<T>.Failure("No matching record found.");
                }

                _logger.LogInformation("[Repository] GetOneByKeyAndValue - Found matching entity where {Key}={Value} in {EntityName}", key, value, typeof(T).Name);
                return ServicesResult<T>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Repository] GetOneByKeyAndValue - Error occurred while querying {EntityName} where {Key}={Value}", typeof(T).Name, key, value);
                return ServicesResult<T>.Failure("An error occurred while retrieving data.");
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
        public async Task<ServicesResult<T>> GetOneByKeyAndValue(Dictionary<string, string> value, bool useAndOperator, CancellationToken cancellationToken = default)
        {
            // Validate input parameters
            if (value == null || value.Count == 0)
            {
                _logger.LogWarning("[Repository] GetOneByKeyAndValue - Invalid parameter: value dictionary is empty.");
                return ServicesResult<T>.Failure("Value is null or empty.");
            }

            _logger.LogInformation("[Repository] GetOneByKeyAndValue - Querying entity {EntityName} with conditions: {Conditions}, Use AND: {UseAnd}",
                typeof(T).Name, string.Join(", ", value.Select(kv => $"{kv.Key}={kv.Value}")), useAndOperator);

            // Check if all properties exist in type T
            foreach (var key in value.Keys)
            {
                if (typeof(T).GetProperty(key) == null)
                {
                    _logger.LogWarning("[Repository] GetOneByKeyAndValue - Property '{Key}' not found in {EntityName}", key, typeof(T).Name);
                    return ServicesResult<T>.Failure($"Property '{key}' not found in {typeof(T).Name}.");
                }
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

                if (result == null)
                {
                    _logger.LogInformation("[Repository] GetOneByKeyAndValue - No record found for conditions: {Conditions}, Use AND: {UseAnd}",
                        string.Join(", ", value.Select(kv => $"{kv.Key}={kv.Value}")), useAndOperator);
                    return ServicesResult<T>.Failure("No matching record found.");
                }

                _logger.LogInformation("[Repository] GetOneByKeyAndValue - Found matching entity {EntityName} with conditions: {Conditions}",
                    typeof(T).Name, string.Join(", ", value.Select(kv => $"{kv.Key}={kv.Value}")));

                return ServicesResult<T>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Repository] GetOneByKeyAndValue - Error while querying {EntityName} with conditions: {Conditions}, Use AND: {UseAnd}",
                    typeof(T).Name, string.Join(", ", value.Select(kv => $"{kv.Key}={kv.Value}")), useAndOperator);
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
            {
                _logger.LogWarning("[Repository] GetManyByKeyAndValue - Invalid parameter: key is null or empty.");
                return ServicesResult<IEnumerable<T>>.Failure("Key cannot be null or empty.");
            }

            if (value == null)
            {
                _logger.LogWarning("[Repository] GetManyByKeyAndValue - Invalid parameter: value is null for key '{Key}'.", key);
                return ServicesResult<IEnumerable<T>>.Failure("Value cannot be null.");
            }

            _logger.LogInformation("[Repository] GetManyByKeyAndValue - Querying {EntityName} where {Key} = {Value}.",
                typeof(T).Name, key, value);

            try
            {
                // Ensure the property exists in the entity type
                var property = typeof(T).GetProperty(key);
                if (property == null)
                {
                    _logger.LogWarning("[Repository] GetManyByKeyAndValue - Property '{Key}' not found in {EntityName}.", key, typeof(T).Name);
                    return ServicesResult<IEnumerable<T>>.Failure($"Property '{key}' not found in {typeof(T).Name}.");
                }

                // Execute the query
                var response = await _dbSet
                    .AsNoTracking()
                    .Where(x => EF.Property<TKey>(x, key).Equals(value))
                    .ToListAsync();

                if (!response.Any())
                {
                    _logger.LogInformation("[Repository] GetManyByKeyAndValue - No records found for {EntityName} where {Key} = {Value}.",
                        typeof(T).Name, key, value);
                }
                else
                {
                    _logger.LogInformation("[Repository] GetManyByKeyAndValue - Found {Count} records for {EntityName} where {Key} = {Value}.",
                        response.Count, typeof(T).Name, key, value);
                }

                return ServicesResult<IEnumerable<T>>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Repository] GetManyByKeyAndValue - Error while querying {EntityName} where {Key} = {Value}.",
                    typeof(T).Name, key, value);
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
        public async Task<ServicesResult<IEnumerable<T>>> GetManyByKeyAndValue(
            Dictionary<string, string> value,
            bool useAndOperator,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            // Validate input parameters
            if (value == null || value.Count == 0)
            {
                _logger.LogWarning("[Repository] GetManyByKeyAndValue - Empty filter conditions. Returning empty result.");
                return ServicesResult<IEnumerable<T>>.Success(Enumerable.Empty<T>());
            }

            if (pageNumber < 1 || pageSize < 1)
            {
                _logger.LogWarning("[Repository] GetManyByKeyAndValue - Invalid pagination params: PageNumber={PageNumber}, PageSize={PageSize}", pageNumber, pageSize);
                return ServicesResult<IEnumerable<T>>.Failure("Invalid page number or page size. Both must be greater than zero.");
            }

            foreach (var key in value.Keys)
            {
                if (typeof(T).GetProperty(key) == null)
                {
                    _logger.LogWarning("[Repository] GetManyByKeyAndValue - Property '{Key}' not found in {EntityName}.", key, typeof(T).Name);
                    return ServicesResult<IEnumerable<T>>.Failure($"Property '{key}' not found in {typeof(T).Name}.");
                }
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

                // Count total records before pagination
                int totalRecords = await query.CountAsync(cancellationToken);

                var results = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("[Repository] GetManyByKeyAndValue - Query completed. Entity={EntityName}, Filters={FilterCount}, UseAndOperator={UseAndOperator}, Page={PageNumber}, PageSize={PageSize}, TotalRecords={TotalRecords}, Retrieved={RetrievedCount}",
                    typeof(T).Name, value.Count, useAndOperator, pageNumber, pageSize, totalRecords, results.Count);

                return ServicesResult<IEnumerable<T>>.Success(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Repository] GetManyByKeyAndValue - Error querying {EntityName} with filters.", typeof(T).Name);
                return ServicesResult<IEnumerable<T>>.Failure("An error occurred while retrieving data.");
            }
        }
        #endregion



    }
}
