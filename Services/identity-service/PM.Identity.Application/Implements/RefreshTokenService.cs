using PM.Identity.Application.Interfaces;
using PM.Identity.Domain.Entities;
using PM.Identity.Infrastructure.Data;
using PM.Shared.Dtos;
using PM.Shared.Persistence.Interfaces;

namespace PM.Identity.Application.Implements
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly AuthDbContext _context;
        private readonly IUnitOfWork<AuthDbContext> _unitOfWork;
        public RefreshTokenService(AuthDbContext context, IUnitOfWork<AuthDbContext> unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }
        public async Task<ServiceResult<bool>> RevokeAllActiveTokensByUserIdAsync(string userId)
        {
            var tokens = await _unitOfWork.RefreshTokenRepository.GetManyAsync("UserId", userId);
            if (!tokens.Data!.Any())
                return ServiceResult<bool>.Success(true);
            var checkTokenUserIsNotRevoke = tokens.Data!.Where(x => x.UserId == userId && x.IsRevoke == false).ToList();

            if (checkTokenUserIsNotRevoke.Count == 0)
                return ServiceResult<bool>.Success(true);
            foreach (var token in checkTokenUserIsNotRevoke)
            {
                Dictionary<string, object> keyValuePairs = new Dictionary<string, object>()
                {
                    {"IsRevoke", true}
                };
                return await UpdateTokenFieldsAsync(token, keyValuePairs);
            }
            return ServiceResult<bool>.Success(true);
        }
        public async Task<ServiceResult<bool>> CreateRefreshTokenAsync(RefreshToken refreshToken)
        {
            var addResponse = await _unitOfWork.ExecuteTransactionAsync(async () =>
            {
                await _unitOfWork.RefreshTokenRepository.AddAsync(refreshToken);
            });
            if (addResponse.Status)
            {
                return ServiceResult<bool>.Success(true);
            }
            else
            {
                return ServiceResult<bool>.Failure("Failed to add refresh token.");
            }
        }
        public async Task<ServiceResult<bool>> UpdateTokenFieldsAsync(RefreshToken refreshToken, Dictionary<string, object> keyValuePairs)
        {
            var updateResponse = await _unitOfWork.ExecuteTransactionAsync(async () =>
            {
                await _unitOfWork.RefreshTokenRepository.PacthAsync(refreshToken, keyValuePairs);
            });
            if (!updateResponse.Status)
            {
                return ServiceResult<bool>.Failure("Failed to update refresh token.");
            }
            return ServiceResult<bool>.Success(true);
        }
        public async Task<ServiceResult<RefreshToken>> GetRefreshTokenByToken(string token)
        {
            var refreshToken = await _unitOfWork.RefreshTokenRepository.GetOneAsync("Token",token);
            if (refreshToken.Status == false)
            {
               return ServiceResult<RefreshToken>.Failure("Failed to get refresh token.");

            }
            if (refreshToken.Data == null)
            {
                return ServiceResult<RefreshToken>.Failure("Refresh token not found.");
            }
            return ServiceResult<RefreshToken>.Success(refreshToken.Data);
        }
        public async Task<ServiceResult<RefreshToken>> GetRefreshTokenByUserId(string userId)
        {
            var refreshToken = await _unitOfWork.RefreshTokenRepository.GetOneAsync("UserId", userId);
            if (refreshToken.Status == false)
            {
                return ServiceResult<RefreshToken>.Failure("Failed to get refresh token.");
            }
            if (refreshToken.Data == null)
            {
                return ServiceResult<RefreshToken>.Failure("Refresh token not found.");
            }
            return ServiceResult<RefreshToken>.Success(refreshToken.Data);
        }
    }
}
