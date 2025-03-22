using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces.Services;

namespace PM.Infrastructer.Implements.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthServices> _logger;
        public AuthServices(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, ILogger<AuthServices> logger)
        {
            
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }
        #region
        public async Task<ServicesResult<bool>> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("");
                return ServicesResult<bool>.Failure("");
            }
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("");
                    return ServicesResult<bool>.Failure("");
                }
                var checkPasswork = await _userManager.CheckPasswordAsync(user, password);
                if (checkPasswork)
                {
                    _logger.LogInformation("");
                    return ServicesResult<bool>.Success(true);
                }
                _logger.LogWarning("");
                return ServicesResult<bool>.Failure("");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("");
                return ServicesResult<bool>.Failure("");
            }
        }
        #endregion

        #region
        public async Task<ServicesResult<bool>> Register(string email, string username, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("");
                return ServicesResult<bool>.Failure("");
            }
            try
            {
                var user = new User()
                {
                    UserName = username,
                    Email = email,
                };
                var register = await _userManager.CreateAsync(user, password);
                if (register.Succeeded)
                {
                    _logger.LogInformation("");
                    return ServicesResult<bool>.Success(true);
                }
                _logger.LogWarning("");
                return ServicesResult<bool>.Failure("");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("");
                return ServicesResult<bool>.Failure("");
            }
        }
        #endregion
    }
}
