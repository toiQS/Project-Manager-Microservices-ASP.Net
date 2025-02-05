using System.Linq.Expressions;

namespace PM.Domain.Interfaces
{
    public interface IRepository<T, TKey> where T : class where TKey : notnull
    {
        /// <summary>
        /// Lấy tất cả dữ liệu.
        /// </summary>
        Task<ServicesResult<IEnumerable<T>>> GetAllAsync();

        /// <summary>
        /// Thêm mới một phần tử và trả về chính phần tử đã thêm.
        /// </summary>
        Task<ServicesResult<T>> AddAsync(T entity);

        /// <summary>
        /// Cập nhật phần tử và trả về đối tượng sau khi cập nhật.
        /// </summary>
        Task<ServicesResult<T>> UpdateAsync(T entity);

        /// <summary>
        /// Xóa phần tử theo khóa chính.
        /// </summary>
        Task<ServicesResult<bool>> DeleteAsync(TKey primaryKey);

        /// <summary>
        /// Kiểm tra sự tồn tại của bản ghi.
        /// </summary>
        Task<bool> ExistsAsync(TKey primaryKey);

        /// <summary>
        /// Tìm kiếm một phần tử theo điều kiện.
        /// </summary>
        Task<ServicesResult<T>> FindOneAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Tìm kiếm danh sách phần tử theo điều kiện.
        /// </summary>
        Task<ServicesResult<IEnumerable<T>>> FindManyAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Lấy dữ liệu có phân trang.
        /// </summary>
        Task<ServicesResult<(IEnumerable<T> Items, int TotalCount)>> GetPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Lưu thay đổi vào database.
        /// </summary>
        Task<ServicesResult<bool>> SaveChangesAsync();
    }
}
