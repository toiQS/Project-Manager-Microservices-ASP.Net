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
        
        Task<bool> ExistAsync(string key, TKey value);

        /// <summary>
        /// Lấy đơn dữ liệu bằng thuộc tính và giá trị
        /// </summary>
        Task<ServicesResult<T>> GetOneByKeyAndValue(string key, TKey value);

        /// <summary>
        /// Lấy đa dữ liệu bằng thuộc tính và giá trị
        /// </summary>
        Task<ServicesResult<IEnumerable<T>>> GetManyByKeyAndValue(string key, TKey value);

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
