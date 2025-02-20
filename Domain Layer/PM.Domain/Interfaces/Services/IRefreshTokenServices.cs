using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    public interface IRefreshTokenServices
    {
        // thêm token vào data
        // kiểm tra token
        // làm mới token
        // hủy token
        // thông tin token
        public Task<ServicesResult<string>> SaveToken(string userId, string token);
        public Task<ServicesResult<string>> CheckStatusToken(string userId);
        public Task<ServicesResult<string>> RefreshToken(string userId);
        public Task<ServicesResult<string>> CancelToken(string userId);
    }
}
