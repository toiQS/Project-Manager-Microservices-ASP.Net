using PM.Identity.Domain.Entities;
using PM.Shared.Dtos;

namespace PM.Identity.Application.Interfaces.Services
{
    public interface IRefreshTokenService
    {
        public Task<ServiceResult<bool>> RevokeAllActiveTokensByUserIdAsync(string userId);
        public Task<ServiceResult<bool>> CreateRefreshTokenAsync(RefreshToken refreshToken);
        public Task<ServiceResult<bool>> UpdateTokenFieldsAsync(RefreshToken refreshToken, Dictionary<string, object> keyValuePairs);
        public Task<ServiceResult<RefreshToken>> GetRefreshTokenByToken(string token);
    }
}
