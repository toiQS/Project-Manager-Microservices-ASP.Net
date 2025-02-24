using Microsoft.AspNetCore.Mvc;
using PM.Application.Interfaces;
using PM.Domain.Models.auths;

namespace AuthAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthLogic _authLogic;

        public AuthController(IAuthLogic authLogic)
        {
            _authLogic = authLogic;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token along with a refresh token.
        /// </summary>
        /// <param name="loginModel">The login credentials.</param>
        /// <returns>An IActionResult with login result.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            return await _authLogic.Login(loginModel);
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerModel">The registration details.</param>
        /// <returns>An IActionResult with registration result.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            return await _authLogic.Register(registerModel);
        }

        /// <summary>
        /// Logs out the user, invalidating the provided token.
        /// </summary>
        /// <param name="token">The JWT token to log out.</param>
        /// <returns>An IActionResult with logout result.</returns>
        [HttpPost("logout")]
        public async Task<IActionResult> LogOut([FromQuery] string token)
        {
            return await _authLogic.LogOut(token);
        }

        /// <summary>
        /// Initiates the forgot password process for a user.
        /// </summary>
        /// <param name="model">The model containing the user's email or other identifying info.</param>
        /// <returns>An IActionResult with forgot password result.</returns>
        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            return await _authLogic.ForgotPassword(model);
        }
    }
}
