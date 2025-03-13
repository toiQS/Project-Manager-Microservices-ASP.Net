namespace PM.Domain.Interfaces
{
    public interface IQueryRepository<T, TKey> where T : class where TKey : notnull
    {
        // Truy vấn dữ liệu (Query Methods)
        public Task<ServicesResult<IEnumerable<T>>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        public Task<ServicesResult<T>> GetOneByKeyAndValue(string key, TKey value);
        public Task<ServicesResult<T>> GetOneByKeyAndValue(Dictionary<string, TKey> value, bool useAndOperator, CancellationToken cancellationToken = default);
        public Task<ServicesResult<IEnumerable<T>>> GetManyByKeyAndValue(string key, TKey value);
        public Task<ServicesResult<IEnumerable<T>>> GetManyByKeyAndValue(Dictionary<string, TKey> value, bool useAndOperator, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
