using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.Logging;
using PM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PM.Persistence.Implements
{
    public class Repository<T, TKey> where T : class where TKey : notnull
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly ILogger<Repository<T, TKey>> _logger;
        public Repository(ApplicationDbContext context, ILogger<Repository<T,TKey>> logger)
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
                var response = await _dbSet
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
                Expression<Func<T, bool>> combinedExpression = null;

                foreach (var condition in value)
                {
                    var parameter = Expression.Parameter(typeof(T), "x");
                    var property = Expression.Property(parameter, condition.Key);
                    var constant = Expression.Constant(condition.Value);
                    var equalsExpression = Expression.Equal(property, constant);
                    var lambda = Expression.Lambda<Func<T, bool>>(equalsExpression, parameter);

                    // Kết hợp các biểu thức với AND hoặc OR
                    if (combinedExpression == null)
                    {
                        combinedExpression = lambda;
                    }
                    else
                    {
                        combinedExpression = useAndOperator
                            ? Expression.Lambda<Func<T, bool>>(Expression.AndAlso(combinedExpression.Body, lambda.Body), parameter)
                            : Expression.Lambda<Func<T, bool>>(Expression.OrElse(combinedExpression.Body, lambda.Body), parameter);
                    }
                }

                if (combinedExpression != null)
                {
                    query = query.Where(combinedExpression);
                }
                // Retrieve the first matching result
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

        //public Task<ServicesResult<T>> GetOneByKeyAndValue(Dictionary<string, TKey> value, bool useAndOperator, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes)
        //{
        //    if(value == null || value.Count == 0)
        //    {
        //        return Task.FromResult(ServicesResult<T>.Failure("Value is null or empty."));
        //    }
        //    try
        //    {
        //        var query = _dbSet.AsQueryable();
        //        Expression<Func<T, bool>> combinedExpression = null;
                
        //    }
        //}
        //public Task<ServicesResult<IEnumerable<T>>> GetManyByKeyAndValue(Dictionary<string, TKey> value, bool useAndOperator, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        //public Task<ServicesResult<IEnumerable<T>>> GetManyByKeyAndValue(Dictionary<string, TKey> value, bool useAndOperator, int pageNumber, int pageSize, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);

        //// Thêm, cập nhật, xóa dữ liệu (Mutation Methods)
        //public Task<ServicesResult<bool>> AddAsync(IEnumerable<T> arr, T entity, CancellationToken cancellationToken = default);
        //public Task<ServicesResult<bool>> UpdateAsync(IEnumerable<T> arr, T entity, CancellationToken cancellationToken = default);
        //public Task<ServicesResult<bool>> PatchAsync(IEnumerable<T> arr, TKey primaryKey, Dictionary<string, object> updateValue, CancellationToken cancellationToken = default);
        //public Task<ServicesResult<bool>> DeleteAsync(TKey primaryKey, CancellationToken cancellationToken = default);
        //public Task<ServicesResult<bool>> DeleteManyAsync(Dictionary<string, TKey> value, bool useAndOperator, CancellationToken cancellationToken = default);

        //// Transaction & Caching
        //public Task<ServicesResult<bool>> ExecuteTransactionAsync(Func<Task> transactionOperations, CancellationToken cancellationToken = default);
        //public Task<ServicesResult<T>> GetCachedAsync(TKey id, CancellationToken cancellationToken = default);

        //// Event-driven & SaveChanges
        //public Task<ServicesResult<bool>> PublishEventAsync(string eventName, object eventData, CancellationToken cancellationToken = default);
        //public Task<ServicesResult<bool>> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
