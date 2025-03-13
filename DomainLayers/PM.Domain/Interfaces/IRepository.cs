namespace PM.Domain.Interfaces
{
    public interface IRepository<T, TKey> where T : class where TKey : notnull
    {
        // Truy vấn dữ liệu (Query Methods)
        public Task<ServicesResult<IEnumerable<T>>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        public Task<ServicesResult<T>> GetOneByKeyAndValue(Dictionary<string, TKey> value, bool useAndOperator, CancellationToken cancellationToken = default);
        public Task<ServicesResult<IEnumerable<T>>> GetManyByKeyAndValue(Dictionary<string, TKey> value, bool useAndOperator, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        // Thêm, cập nhật, xóa dữ liệu (Mutation Methods)
        public Task<ServicesResult<bool>> AddAsync(List<T> arr, T entity, CancellationToken cancellationToken = default);
        public Task<ServicesResult<bool>> UpdateAsync(List<T> arr, T entity, CancellationToken cancellationToken = default);
        public Task<ServicesResult<bool>> PatchAsync(List<T> arr, TKey primaryKey, Dictionary<string, object> updateValue, CancellationToken cancellationToken = default);
        public Task<ServicesResult<bool>> DeleteAsync(TKey primaryKey, CancellationToken cancellationToken = default);
        public Task<ServicesResult<bool>> DeleteManyAsync(Dictionary<string, TKey> value, bool useAndOperator, CancellationToken cancellationToken = default);

    }
}
