using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PM.Application.Interfaces;
using PM.Application.Models.users;
using PM.Domain.Interfaces.Services;
using PM.Domain.Models.users;
using PM.Infrastructers.Interfaces;
using System.Transactions;

namespace PM.Application.Implements
{
    public class UserLogic : ControllerBase, IUserLogic
    {
        private readonly IUserServices _userServices;
        private readonly IJwtServices _jwtServices;
        private readonly ILogger<UserLogic> _logger;

        public UserLogic(IUserServices userServices, IJwtServices jwtServices, ILogger<UserLogic> logger)
        {
            _userServices = userServices;
            _jwtServices = jwtServices;
            _logger = logger;
        }

        #region GetDetailUserToken
        /// <summary>
        /// Retrieves user details based on the provided token.
        /// </summary>
        public IActionResult GetDetailUserToken(string token)
        {
            var response = _jwtServices.ParseToken(token);
            if (!response.Status)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Data);
        }
        #endregion

        #region GetDetailUserIdentity
        /// <summary>
        /// Retrieves detailed user information based on the user ID.
        /// </summary>
        public async Task<IActionResult> GetDetailUserIdentity(string userId)
        {
            var response = await _userServices.GetDetailUser(userId);
            if (!response.Status)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Data);
        }
        #endregion

        #region UpdateUser
        /// <summary>
        /// Updates user information.
        /// </summary>
        public async Task<IActionResult> UpdateUser(UpdateUserModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid model state");
            try
            {
                var responseToken = _jwtServices.ParseToken(model.Token);
                if (!responseToken.Status || responseToken.Data?.UserId == null)
                {
                    return BadRequest(responseToken.Message);
                }

                var updateUser = new UpdateAppUser()
                {
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PathImage = model.PathImage,
                    UserName = model.UserName,
                    Phone = model.Phone,
                };

                var responseUpdate = await _userServices.UpdateUser(responseToken.Data.UserId, updateUser);
                if (!responseUpdate.Status)
                {
                    return BadRequest(responseUpdate.Message);
                }
                return Ok(responseUpdate.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region UpdateAvatar
        /// <summary>
        /// Updates user avatar.
        /// </summary>
        public async Task<IActionResult> UpdateAvatar(UpdateAvataModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid model state");
            try
            {
                var responseToken = _jwtServices.ParseToken(model.Token);
                if (!responseToken.Status || responseToken.Data?.UserId == null)
                {
                    return BadRequest(responseToken.Message);
                }

                var responseUpdate = await _userServices.UpdateAvata(responseToken.Data.UserId, model.Avata);
                if (!responseUpdate.Status)
                {
                    return BadRequest(responseUpdate.Message);
                }
                return Ok(responseUpdate.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating avatar.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region ChangePassword
        /// <summary>
        /// Changes user password.
        /// </summary>
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid model state");
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var changePassword = new ChangePasswordUser()
                    {
                        Email = model.Email,
                        NewPassword = model.NewPassword,
                        OldPassword = model.OldPassword,
                    };
                    var response = await _userServices.ChangePassword(changePassword);
                    if (!response.Status)
                    {
                        return BadRequest(response.Message);
                    }
                    scope.Complete();
                    return Ok(response.Data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing password.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion
    }
}
