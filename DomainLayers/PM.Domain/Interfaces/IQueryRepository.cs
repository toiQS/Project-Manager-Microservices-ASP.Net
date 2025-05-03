namespace PM.Domain.Interfaces
{
    /// <summary>
    /// Generic repository interface for querying data.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the entity's key.</typeparam>
    public interface IQueryRepository<T, TKey> where T : class where TKey : notnull
    {
        /// <summary>
        /// Retrieves all records with pagination.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of records per page.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A collection of entities wrapped in a service result.</returns>
        public Task<ServicesResult<IEnumerable<T>>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a single record by a specified key and value.
        /// </summary>
        /// <param name="key">The key to search by.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns>The entity that matches the provided key and value.</returns>
        public Task<ServicesResult<T>> GetOneByKeyAndValue(string key, TKey value);

        /// <summary>
        /// Retrieves a single record based on multiple key-value pairs.
        /// </summary>
        /// <param name="value">The dictionary of key-value pairs to search by.</param>
        /// <param name="useAndOperator">Indicates whether to use AND or OR between conditions.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The entity that matches the provided conditions.</returns>
        public Task<ServicesResult<T>> GetOneByKeyAndValue(Dictionary<string, string> value, bool useAndOperator, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves multiple records by a specified key and value.
        /// </summary>
        /// <param name="key">The key to search by.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns>A collection of entities that match the provided key and value.</returns>
        public Task<ServicesResult<IEnumerable<T>>> GetManyByKeyAndValue(string key, TKey value);

        /// <summary>
        /// Retrieves multiple records based on multiple key-value pairs with pagination.
        /// </summary>
        /// <param name="keyValuePairs">The dictionary of key-value pairs to search by.</param>
        /// <param name="useAndOperator">Indicates whether to use AND or OR between conditions.</param>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of records per page.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A collection of entities that match the provided conditions.</returns>
        public Task<ServicesResult<IEnumerable<T>>> GetManyByKeyAndValue(Dictionary<string, string> keyValuePairs, bool useAndOperator, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}