namespace PM.Domain.Interfaces
{
    public interface ICommandRepository<T, TKey> where T : class where TKey : notnull
    {
        // Thêm, cập nhật, xóa dữ liệu (Mutation Methods)
        public Task<ServicesResult<bool>> AddAsync(List<T> arr, T entity, CancellationToken cancellationToken = default);
        public Task<ServicesResult<bool>> UpdateAsync(List<T> arr, T entity, CancellationToken cancellationToken = default);
        public Task<ServicesResult<bool>> PatchAsync(List<T> arr, TKey primaryKey, Dictionary<string, object> updateValue, CancellationToken cancellationToken = default);
        public Task<ServicesResult<bool>> DeleteAsync(TKey primaryKey, CancellationToken cancellationToken = default);
        public Task<ServicesResult<bool>> DeleteManyAsync(Dictionary<string, TKey> value, bool useAndOperator, CancellationToken cancellationToken = default);
    }
}
