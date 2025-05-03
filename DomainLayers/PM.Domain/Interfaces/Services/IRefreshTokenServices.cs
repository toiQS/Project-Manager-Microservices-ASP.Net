using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    public interface IRefreshTokenServices
    {
        public Task<ServicesResult<bool>> AddAsync(RefreshToken refreshToken);
        public Task<ServicesResult<bool>> RemoveAsync(string userId);
    }
}
