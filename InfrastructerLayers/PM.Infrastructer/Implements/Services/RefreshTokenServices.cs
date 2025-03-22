using Microsoft.Extensions.Logging;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;

namespace PM.Infrastructer.Implements.Services
{
    public class RefreshTokenServices
    {
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly ILogger<RefreshTokenServices> _logger;
        public RefreshTokenServices(IAuthUnitOfWork authUnitOfWork, ILogger<RefreshTokenServices> logger)
        {
            _authUnitOfWork = authUnitOfWork;
            _logger = logger;
        }

        public async Task<ServicesResult<bool>> AddAsync(RefreshToken refreshToken)
        {
            if (refreshToken == null)
            {
                _logger.LogError("");
                return ServicesResult<bool>.Failure("");
            }
            try
            {
                var tokens = await _authUnitOfWork.RefreshTokenQueryRepoitory.GetManyByKeyAndValue("UserId", refreshToken.UserId);
                if (tokens.Data!.Any(x => x.IsExpired == true))
                {
                    _logger.LogError("");
                    return ServicesResult<bool>.Failure("");
                }
                var addResponse = await _authUnitOfWork.RefreshTokenCommandRepoitory.AddAsync(tokens.Data!.ToList(), refreshToken);
                if (addResponse.Status == false)
                {
                    _logger.LogError("");
                    return ServicesResult<bool>.Failure("");
                }
                _logger.LogInformation("");
                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError("");
                return ServicesResult<bool>.Failure("");
            }
        }
        public async Task<ServicesResult<bool>> RemoveAsync(string tokenId)
        {
            if (tokenId == null)
            {
                _logger.LogError("");
                return ServicesResult<bool>.Failure("");

            }
            try
            {
                var deleteResponse = await _authUnitOfWork.RefreshTokenCommandRepoitory.DeleteAsync(tokenId);
                if (deleteResponse.Status == false)
                {
                    _logger.LogError("");
                    return ServicesResult<bool>.Failure("");
                }
                _logger.LogInformation("");
                return ServicesResult<bool>.Success(true);

            }
            catch (Exception ex)
            {
                _logger.LogError("");
                return ServicesResult<bool>.Failure("");
            }
        }
    }
}
