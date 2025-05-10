namespace PM.Shared.Infrastructure.Interfaces
{
    public interface IAPIService<T> where T : class
    {
        Task<T> APIsGetAsync(string endpoint);
        Task<T> APIsPostAsync(string endpoint, object data);
        Task<T> APIsPutAsync(string endpoint, object data);
        Task<T> APIsPatchAsync(string endpoint, object data);
        Task<T> APIsDeleteAsync(string endpoint, object data);
    }

}
