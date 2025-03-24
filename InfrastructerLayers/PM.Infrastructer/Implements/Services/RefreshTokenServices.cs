using Microsoft.Extensions.Logging;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;

namespace PM.Infrastructure.Implements.Services
{
    public class RefreshTokenServices : IRefreshTokenServices
    {
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly ILogger<RefreshTokenServices> _logger;

        public RefreshTokenServices(IAuthUnitOfWork authUnitOfWork, ILogger<RefreshTokenServices> logger)
        {
            _authUnitOfWork = authUnitOfWork;
            _logger = logger;
        }

        #region Add Refresh Token
        public async Task<ServicesResult<bool>> AddAsync(RefreshToken refreshToken)
        {
            if (refreshToken == null)
            {
                _logger.LogError("[Service] AddAsync failed: RefreshToken object is null.");
                return ServicesResult<bool>.Failure("RefreshToken cannot be null.");
            }

            _logger.LogInformation("[Service] Adding refresh token for UserId={UserId}", refreshToken.UserId);
            var tokens = await _authUnitOfWork.RefreshTokenQueryRepoitory.GetManyByKeyAndValue("UserId", refreshToken.UserId);

            if (tokens.Data != null && tokens.Data.Any(x => x.IsExpired))
            {
                _logger.LogError("[Service] AddAsync failed: UserId={UserId} has expired tokens.", refreshToken.UserId);
                return ServicesResult<bool>.Failure("User has expired tokens.");
            }

            var addResponse = await _authUnitOfWork.RefreshTokenCommandRepoitory.AddAsync(tokens.Data?.ToList() ?? new List<RefreshToken>(), refreshToken);

            if (!addResponse.Status)
            {
                _logger.LogError("[Service] AddAsync failed: Database error for UserId={UserId}", refreshToken.UserId);
                return ServicesResult<bool>.Failure("Database error.");
            }

            _logger.LogInformation("[Service] Successfully added refresh token for UserId={UserId}", refreshToken.UserId);
            return ServicesResult<bool>.Success(true);
        }
        #endregion

        #region Remove Refresh Token
        public async Task<ServicesResult<bool>> RemoveAsync(string tokenId)
        {
            if (string.IsNullOrWhiteSpace(tokenId))
            {
                _logger.LogError("[Service] RemoveAsync failed: TokenId is null or empty.");
                return ServicesResult<bool>.Failure("Token ID cannot be null or empty.");
            }

            _logger.LogInformation("[Service] Removing refresh token: TokenId={TokenId}", tokenId);
            var deleteResponse = await _authUnitOfWork.RefreshTokenCommandRepoitory.DeleteAsync(tokenId);

            if (!deleteResponse.Status)
            {
                _logger.LogError("[Service] RemoveAsync failed: Database error for TokenId={TokenId}", tokenId);
                return ServicesResult<bool>.Failure("Database error.");
            }

            _logger.LogInformation("[Service] Successfully removed refresh token: TokenId={TokenId}", tokenId);
            return ServicesResult<bool>.Success(true);
        }
        #endregion
    }
}