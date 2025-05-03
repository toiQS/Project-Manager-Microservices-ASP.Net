using PM.Shared.Dtos.auths;

namespace PM.Shared.Handle.Interfaces
{
    public interface IAPIService<T> where T : class
    {
        Task<ServiceResult<T>> APIsGetAsync(string endpoint);
        Task<ServiceResult<T>> APIsPostAsync(string endpoint, object inputData);
    }
}
