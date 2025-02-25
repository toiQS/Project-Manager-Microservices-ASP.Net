using Microsoft.AspNetCore.Mvc;
using PM.Application.Interfaces;
using PM.Application.Models.users;
using PM.Domain.Models.users;

namespace ManagerAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserLogic _userLogic;
        
        public UserController(IUserLogic userLogic)
        {
            _userLogic = userLogic;
           
        }
        #region GetUserDetail
        /// <summary>
        /// Retrieves user details using a token.
        /// </summary>
        [HttpGet("user-detail-token")]
        public IActionResult GetUserDetail(string token)
        {
            return _userLogic.GetDetailUserToken(token);
        }
        #endregion

        #region GetOtherUserDetail
        /// <summary>
        /// Retrieves details of another user using userId.
        /// </summary>
        [HttpGet("another-user-detail-identity")]
        public async Task<IActionResult> GetOtherUserDetail(string userId)
        {
            return await _userLogic.GetDetailUserIdentity(userId);
        }
        #endregion

        #region UpdateUser
        /// <summary>
        /// Updates user details.
        /// </summary>
        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser(string token, [FromBody] UpdateUserModel model)
        {
            return await _userLogic.UpdateUser(model);
        }
        #endregion

        #region UpdateAvatar
        /// <summary>
        /// Updates user avatar.
        /// </summary>
        [HttpPut("update-avatar")]
        public async Task<IActionResult> UpdateAvatar([FromBody] UpdateAvataModel model)
        {
            return await _userLogic.UpdateAvatar(model);
        }
        #endregion

        #region ChangePassword
        /// <summary>
        /// Changes user password.
        /// </summary>
        [HttpPatch("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            return await _userLogic.ChangePassword(model);
        }
        #endregion

    }
}
