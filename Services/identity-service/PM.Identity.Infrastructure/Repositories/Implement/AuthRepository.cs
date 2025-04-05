using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PM.Identity.Domain.Entities;
using PM.Identity.Infrastructure.Repositories.Interface;
using PM.Shared.Dtos;

namespace PM.Identity.Infrastructure.Repositories.Implement
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthRepository> _logger;
        private readonly SignInManager<User> _signInManager;
        public AuthRepository(UserManager<User> userManager,
                              RoleManager<IdentityRole> roleManager,
                              ILogger<AuthRepository> logger,
                              SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _signInManager = signInManager;
        }
        public async Task<ServiceResult<bool>> Login(string email, string password)
        {
            try
            {
                _logger.LogInformation("Login attempt for user: {email}", email);
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("User not found");
                    return ServiceResult<bool>.Failure("User not found");
                }
                var result = await _userManager.CheckPasswordAsync(user, password);
                if (!result)
                {
                    _logger.LogWarning("Invalid password");
                    return ServiceResult<bool>.Failure("Invalid password");
                }
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login");
                return ServiceResult<bool>.Failure(ex);
            }
        }

        public async Task<ServiceResult<bool>> Register(string email, string username, string password)
        {
            try
            {
                _logger.LogInformation("Registering user: {email}", email);
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    _logger.LogWarning("User already exists");
                    return ServiceResult<bool>.Failure("User already exists");
                }
                var user = new User
                {
                    UserName = username,
                    Email = email,
                    FirstName = "Empty",
                    LastName = "Empty",
                    FullName = "Emtpy",
                    AvatarPath = "Emtpy",
                };
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("User registration failed");
                    return ServiceResult<bool>.Failure("User registration failed");
                }
                var addRoleResult = await _userManager.AddToRoleAsync(user, "Customer");
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration");
                return ServiceResult<bool>.Failure(ex);
            }
        }
        public async Task<ServiceResult<bool>> Logout()
        {
            try
            {
                _logger.LogInformation("Logging out user");
                await _signInManager.SignOutAsync();
                return ServiceResult<bool>.Success(true);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred during logout");
                return ServiceResult<bool>.Failure(ex);
            }
        }

        public async Task<ServiceResult<bool>> ChangePassword(string email, string oldPassword, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if(user == null)
                {
                    _logger.LogWarning("User not found");
                    return ServiceResult<bool>.Failure("User not found");
                }
                var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Password change failed");
                    return ServiceResult<bool>.Failure("Password change failed");
                }
                _logger.LogInformation("Password changed successfully for user: {email}", email);
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during password change");
                return ServiceResult<bool>.Failure(ex);
            }
        }
        
    }
}
