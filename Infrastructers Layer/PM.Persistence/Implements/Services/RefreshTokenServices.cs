using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;

namespace PM.Persistence.Implements.Services
{
    public class RefreshTokenServices : IRefreshTokenServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTokenServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        #region Saves a refresh token for a user and logs the login activity.
        /// <summary>
        /// Saves a refresh token for a user and logs the login activity.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="token">The token to be saved.</param>
        /// <returns>A service result indicating success or failure with a message.</returns>
        public async Task<ServicesResult<string>> SaveToken(string userId, string token)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return ServicesResult<string>.Failure("User ID or token cannot be null or empty.");

            try
            {
                // Retrieve user details
                var userResult = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);
                if (!userResult.Status)
                    return ServicesResult<string>.Failure($"Failed to retrieve user: {userResult.Message}");

                // Create a new refresh token object
                var refreshToken = new RefreshToken
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Token = token,
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    IsExpired = true,
                    IsRevoke = false
                };

                // Save the refresh token
                var tokenResult = await _unitOfWork.RefreshTokenRepository.AddAsync(refreshToken);
                if (!tokenResult.Status)
                    return ServicesResult<string>.Failure($"Failed to save token: {tokenResult.Message}");

                // Log the login action
                var log = new ActivityLog
                {
                    Id = Guid.NewGuid().ToString(),
                    Action = $"{userResult.Data.UserName} logged in to the system.",
                    UserId = userId,
                    ActionDate = DateTime.UtcNow
                };

                var logResult = await _unitOfWork.ActivityLogRepository.AddAsync(log);
                if (!logResult.Status)
                    return ServicesResult<string>.Failure($"Failed to log activity: {logResult.Message}");

                return ServicesResult<string>.Success("Token saved successfully.");
            }
            catch (Exception ex)
            {
                return ServicesResult<string>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }

        #endregion
        #region Checks the status of a user's refresh token. If the token is still valid (has not expired), it revokes it.
        /// <summary>
        /// Checks the status of a user's refresh token. If the token is still valid (has not expired), it revokes it.
        /// </summary>
        /// <param name="userId">The ID of the user whose token status is to be checked.</param>
        /// <returns>A service result with a message indicating the outcome.</returns>
        public async Task<ServicesResult<string>> CheckStatusToken(string userId)
        {
            // Validate input parameter
            if (string.IsNullOrEmpty(userId))
                return ServicesResult<string>.Failure("User ID cannot be null or empty.");

            try
            {
                // Retrieve user details
                var userResult = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);
                if (!userResult.Status)
                    return ServicesResult<string>.Failure($"Failed to retrieve user: {userResult.Message}");

                // Retrieve the refresh token associated with the user
                var tokenResult = await _unitOfWork.RefreshTokenRepository.GetOneByKeyAndValue("UserId", userId);
                if (!tokenResult.Status)
                    return ServicesResult<string>.Failure($"Failed to retrieve token: {tokenResult.Message}");

                // Check if the token is still valid (i.e., not expired)
                if (tokenResult.Data.Expires >= DateTime.UtcNow)
                {
                    // Revoke the token if it is still valid
                    tokenResult.Data.IsRevoke = true;
                    tokenResult.Data.IsExpired = false;

                    var updateResult = await _unitOfWork.RefreshTokenRepository.UpdateAsync(tokenResult.Data);
                    if (!updateResult.Status)
                        return ServicesResult<string>.Failure($"Failed to update token status: {updateResult.Message}");

                    return ServicesResult<string>.Success("Token has been revoked successfully.");
                }

                // If token is expired, just return a success message
                return ServicesResult<string>.Success("Token is already expired.");
            }
            catch (Exception ex)
            {
                return ServicesResult<string>.Failure($"An unexpected error occurred: {ex.Message}");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        #endregion
        #region Refreshes the user's token by updating it with a new token value and extending its expiration.
        /// <summary>
        /// Refreshes the user's token by updating it with a new token value and extending its expiration.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="newToken">The new token to set.</param>
        /// <returns>A service result indicating success or failure with a message.</returns>
        public async Task<ServicesResult<string>> RefreshToken(string userId, string newToken)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newToken))
                return ServicesResult<string>.Failure("User ID and new token cannot be null or empty.");

            try
            {
                // Retrieve user details
                var userResult = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);
                if (!userResult.Status)
                    return ServicesResult<string>.Failure($"Failed to retrieve user: {userResult.Message}");

                // Retrieve the refresh token associated with the user
                var tokenResult = await _unitOfWork.RefreshTokenRepository.GetOneByKeyAndValue("UserId", userId);
                if (!tokenResult.Status)
                    return ServicesResult<string>.Failure($"Failed to retrieve token: {tokenResult.Message}");

                // Update the token details
                tokenResult.Data.Token = newToken;
                tokenResult.Data.Expires = DateTime.UtcNow.AddMinutes(30);
                tokenResult.Data.IsExpired = true;
                tokenResult.Data.IsRevoke = false;

                // Save the updated token
                var updateResult = await _unitOfWork.RefreshTokenRepository.UpdateAsync(tokenResult.Data);
                if (!updateResult.Status)
                    return ServicesResult<string>.Failure($"Failed to update token: {updateResult.Message}");

                return ServicesResult<string>.Success("Token refreshed successfully.");
            }
            catch (Exception ex)
            {
                return ServicesResult<string>.Failure($"An unexpected error occurred: {ex.Message}");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        #endregion
        #region  Cancels (deletes) the refresh token associated with the specified user.
        /// <summary>
        /// Cancels (deletes) the refresh token associated with the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user whose token should be cancelled.</param>
        /// <returns>A service result indicating success or failure with an appropriate message.</returns>
        public async Task<ServicesResult<string>> CancelToken(string userId)
        {
            // Validate input parameter
            if (string.IsNullOrEmpty(userId))
                return ServicesResult<string>.Failure("User ID cannot be null or empty.");

            try
            {
                // Retrieve user details
                var userResult = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);
                if (!userResult.Status)
                    return ServicesResult<string>.Failure($"Failed to retrieve user: {userResult.Message}");

                // Retrieve the refresh token associated with the user
                var tokenResult = await _unitOfWork.RefreshTokenRepository.GetOneByKeyAndValue("UserId", userId);
                if (!tokenResult.Status)
                    return ServicesResult<string>.Failure($"Failed to retrieve token: {tokenResult.Message}");

                // Delete the refresh token
                var deleteResult = await _unitOfWork.RefreshTokenRepository.DeleteAsync(tokenResult.Data.Id);
                if (!deleteResult.Status)
                    return ServicesResult<string>.Failure($"Failed to cancel token: {deleteResult.Message}");

                return ServicesResult<string>.Success("Token cancelled successfully.");
            }
            catch (Exception ex)
            {
                return ServicesResult<string>.Failure($"An unexpected error occurred: {ex.Message}");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        #endregion
    }
}
