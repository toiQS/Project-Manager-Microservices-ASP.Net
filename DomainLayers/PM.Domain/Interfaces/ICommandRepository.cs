namespace PM.Domain.Interfaces
{
    /// <summary>
    /// Generic command repository interface for handling CRUD operations.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <typeparam name="TKey">The type of the primary key.</typeparam>
    public interface ICommandRepository<T, TKey> where T : class where TKey : notnull
    {
        /// <summary>
        /// Adds a new entity to the collection.
        /// </summary>
        /// <param name="arr">The list of entities.</param>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>Result indicating success or failure.</returns>
        public Task<ServicesResult<bool>> AddAsync(List<T> arr, T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing entity in the collection.
        /// </summary>
        /// <param name="arr">The list of entities.</param>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>Result indicating success or failure.</returns>
        public Task<ServicesResult<bool>> UpdateAsync(List<T> arr, T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Patches an entity with specified values.
        /// </summary>
        /// <param name="arr">The list of entities.</param>
        /// <param name="primaryKey">The primary key of the entity.</param>
        /// <param name="updateValue">Dictionary of field-value pairs to update.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>Result indicating success or failure.</returns>
        public Task<ServicesResult<bool>> PatchAsync(List<T> arr, TKey primaryKey, Dictionary<string, object> updateValue, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="primaryKey">The primary key of the entity.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>Result indicating success or failure.</returns>
        public Task<ServicesResult<bool>> DeleteAsync(TKey primaryKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes multiple entities by a key-value pair.
        /// </summary>
        /// <param name="key">The key for filtering.</param>
        /// <param name="valueKey">The value associated with the key.</param>
        /// <returns>Result indicating success or failure.</returns>
        public Task<ServicesResult<bool>> DeleteManyAsync(string key, TKey valueKey);

        /// <summary>
        /// Deletes multiple entities based on a dictionary of key-value pairs.
        /// </summary>
        /// <param name="value">Dictionary of key-value pairs.</param>
        /// <param name="useAndOperator">Indicates whether to use AND or OR logic.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>Result indicating success or failure.</returns>
        public Task<ServicesResult<bool>> DeleteManyAsync(Dictionary<string, TKey> value, bool useAndOperator, CancellationToken cancellationToken = default);
    }
}
