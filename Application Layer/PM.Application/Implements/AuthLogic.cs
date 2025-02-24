using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PM.Application.Interfaces;
using PM.Domain.Interfaces.Services;
using PM.Domain.Models.auths;
using PM.Infrastructers.Interfaces;

namespace PM.Application.Implements
{
    public class AuthLogic : ControllerBase, IAuthLogic
    {
        private readonly IAuthServices _authServices;
        private readonly IJwtServices _jwtServices;
        private readonly IRefreshTokenServices _refreshTokenServices;
        private readonly ILogger<AuthLogic> _logger;

        public AuthLogic(IAuthServices authServices, IJwtServices jwtServices, IRefreshTokenServices refreshTokenServices, ILogger<AuthLogic> logger)
        {
            _authServices = authServices;
            _jwtServices = jwtServices;
            _refreshTokenServices = refreshTokenServices;
            _logger = logger;
        }

        #region Logs in a user, generates a JWT token and refresh token, and returns them.
        /// <summary>
        /// Logs in a user, generates a JWT token and refresh token, and returns them.
        /// </summary>
        /// <param name="loginModel">The login credentials.</param>
        /// <returns>An IActionResult with the token on success or an error message.</returns>
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            try
            {
                // Authenticate the user using the auth service
                var loginResult = await _authServices.LoginAsync(loginModel);
                if (!loginResult.Status || loginResult.Data == null)
                {
                    return BadRequest(loginResult.Message);
                }

                // Generate JWT token for the authenticated user
                var tokenResult = _jwtServices.GenerateToken(loginResult.Data);
                if (!tokenResult.Status)
                {
                    return BadRequest(tokenResult.Message);
                }

                // Save the refresh token for future use
                var refreshTokenResult = await _refreshTokenServices.SaveToken(loginResult.Data.UserId, tokenResult.Data);
                if (!refreshTokenResult.Status)
                {
                    return BadRequest(refreshTokenResult.Message);
                }

                return Ok(new
                {
                    message = "Login success.",
                    token = tokenResult.Data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region Registers a new user.
        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerModel">The registration details.</param>
        /// <returns>An IActionResult indicating the success or failure of the registration.</returns>
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            var registerResult = await _authServices.RegisterAsync(registerModel);
            if (!registerResult.Status)
            {
                return BadRequest(registerResult.Message);
            }
            return Ok(new
            {
                message = "Register success.",
                data = registerResult.Data.ToString()
            });
        }
        #endregion

        #region  Logs out the user by canceling the refresh token.
        /// <summary>
        /// Logs out the user by canceling the refresh token.
        /// </summary>
        /// <param name="token">The JWT token of the user.</param>
        /// <returns>An IActionResult indicating the success or failure of the logout process.</returns>
        public async Task<IActionResult> LogOut(string token)
        {
            var userInfo = _jwtServices.ParseToken(token);
            if (!userInfo.Status)
            {
                return BadRequest(userInfo.Message);
            }

            // Cancel the refresh token
            var cancelTokenResult = await _refreshTokenServices.CancelToken(userInfo.Data.UserId);
            if (!cancelTokenResult.Status)
            {
                return BadRequest(cancelTokenResult.Message);
            }

            // Optionally, perform any additional logout operations via the auth service
            var logOutResult = await _authServices.LogOutAsync();
            if (!logOutResult.Status)
            {
                return BadRequest(logOutResult.Message);
            }

            return Ok("Log out success");
        }
        #endregion

        #region  Initiates the forgot password process.
        /// <summary>
        /// Initiates the forgot password process.
        /// </summary>
        /// <param name="model">The model containing the user's email or identifying information.</param>
        /// <returns>An IActionResult indicating the success or failure of the process.</returns>
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            var forgotPasswordResult = await _authServices.ForgotPassword(model);
            if (!forgotPasswordResult.Status)
            {
                return BadRequest(forgotPasswordResult.Message);
            }
            return Ok("Forgot password success");
        }
        #endregion
    }
}
