using Microsoft.Extensions.Logging;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;

namespace PM.Infrastructure.Implements.Services
{
    public class UserServices : IUserServices
    {
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly ILogger<UserServices> _logger;

        public UserServices(IAuthUnitOfWork authUnitOfWork, ILogger<UserServices> logger)
        {
            _authUnitOfWork = authUnitOfWork;
            _logger = logger;
        }

        #region Get User Details
        public async Task<ServicesResult<User>> GetDetailUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogError("[Service] GetDetailUserAsync failed: userId is null or empty.");
                return ServicesResult<User>.Failure("User ID cannot be null or empty.");
            }

            _logger.LogInformation("[Service] Fetching user details for userId={UserId}", userId);
            var userResponse = await _authUnitOfWork.UserQueryRepository.GetOneByKeyAndValue("Id", userId);

            if (!userResponse.Status)
            {
                _logger.LogError("[Service] User not found for userId={UserId}", userId);
                return ServicesResult<User>.Failure("User not found.");
            }

            _logger.LogInformation("[Service] Successfully fetched user details for userId={UserId}", userId);
            return ServicesResult<User>.Success(userResponse.Data!);
        }
        #endregion

        #region Update User
        public async Task<ServicesResult<bool>> UpdateUserAsync(User user)
        {
            if (user == null)
            {
                _logger.LogError("[Service] UpdateUserAsync failed: User object is null.");
                return ServicesResult<bool>.Failure("User object cannot be null.");
            }

            _logger.LogInformation("[Service] Updating user with Id={UserId}", user.Id);
            var updateResponse = await _authUnitOfWork.UserCommandRepository.UpdateAsync(new List<User> { user }, user);

            if (!updateResponse.Status)
            {
                _logger.LogError("[Service] UpdateUserAsync failed: Database error for UserId={UserId}", user.Id);
                return ServicesResult<bool>.Failure("Database error.");
            }

            _logger.LogInformation("[Service] Successfully updated user with Id={UserId}", user.Id);
            return ServicesResult<bool>.Success(true);
        }
        #endregion

        #region Patch User
        public async Task<ServicesResult<bool>> PatchUserAsync(string userId, User user)
        {
            if (string.IsNullOrWhiteSpace(userId) || user == null)
            {
                _logger.LogError("[Service] PatchUserAsync failed: Invalid input. UserId or User object is null.");
                return ServicesResult<bool>.Failure("Invalid input.");
            }

            _logger.LogInformation("[Service] Patching user with Id={UserId}", userId);
            var parameters = new Dictionary<string, object>
            {
                {"Email", user.Email! },
                {"UserName", user.UserName!},
                {"FirstName", user.FirstName!},
                {"LastName", user.LastName!},
                {"FullName", user.FullName },
                {"PhoneNumber", user.PhoneNumber! }
            };

            var patchResponse = await _authUnitOfWork.UserCommandRepository.PatchAsync(new List<User> { user }, userId, parameters);

            if (!patchResponse.Status)
            {
                _logger.LogError("[Service] PatchUserAsync failed: Database error for UserId={UserId}", userId);
                return ServicesResult<bool>.Failure("Database error.");
            }

            _logger.LogInformation("[Service] Successfully patched user with Id={UserId}", userId);
            return ServicesResult<bool>.Success(true);
        }
        #endregion
    }
}
