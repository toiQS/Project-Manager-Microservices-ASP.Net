using Microsoft.Extensions.Logging;
using PM.Domain.Entities;
using PM.Domain;
using PM.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
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

        #region Login
        public async Task<ServicesResult<bool>> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("[Service] Login failed: Email or password is null or empty.");
                return ServicesResult<bool>.Failure("Email and password are required.");
            }

            _logger.LogInformation("[Service] Attempting login for Email={Email}", email);

            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("[Service] Login failed: User with Email={Email} not found.", email);
                    return ServicesResult<bool>.Failure("Invalid credentials.");
                }

                var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
                if (!isPasswordValid)
                {
                    _logger.LogWarning("[Service] Login failed: Incorrect password for Email={Email}", email);
                    return ServicesResult<bool>.Failure("Invalid credentials.");
                }

                _logger.LogInformation("[Service] Successfully logged in: Email={Email}", email);
                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Service] Login failed: Unexpected error for Email={Email}", email);
                return ServicesResult<bool>.Failure("An unexpected error occurred.");
            }
        }
        #endregion

        #region Register
        public async Task<ServicesResult<bool>> Register(string email, string username, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("[Service] Register failed: Email, username, or password is null or empty.");
                return ServicesResult<bool>.Failure("All fields are required.");
            }

            _logger.LogInformation("[Service] Registering new user: Email={Email}, Username={Username}", email, username);

            try
            {
                var user = new User { UserName = username, Email = email };
                var result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("[Service] Register failed: Could not create user Email={Email}, Errors={Errors}", email, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return ServicesResult<bool>.Failure("Registration failed.");
                }

                _logger.LogInformation("[Service] Successfully registered user: Email={Email}, Username={Username}", email, username);
                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Service] Register failed: Unexpected error for Email={Email}", email);
                return ServicesResult<bool>.Failure("An unexpected error occurred.");
            }
        }
        #endregion

        #region GetUserByEmail
        public async Task<ServicesResult<User>> GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("[Service] GetUserByEmail failed: Email is null or empty.");
                return ServicesResult<User>.Failure("Email is required.");
            }

            _logger.LogInformation("[Service] Retrieving user by Email={Email}", email);

            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("[Service] GetUserByEmail failed: User with Email={Email} not found.", email);
                    return ServicesResult<User>.Failure("User not found.");
                }

                _logger.LogInformation("[Service] Successfully retrieved user: Email={Email}", email);
                return ServicesResult<User>.Success(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Service] GetUserByEmail failed: Unexpected error for Email={Email}", email);
                return ServicesResult<User>.Failure("An unexpected error occurred.");
            }
        }
        #endregion
        #region AddRoleCustomer
        public async Task<ServicesResult<bool>> AddRoleCustomer(User user)
        {
            if (user == null)
            {
                _logger.LogWarning("[Service] AddRoleCustomer failed: User object is null.");
                return ServicesResult<bool>.Failure("User cannot be null.");
            }

            try
            {
                var roleExists = await _roleManager.RoleExistsAsync("Customer");
                if (!roleExists)
                {
                    _logger.LogWarning("[Service] AddRoleCustomer failed: Role 'Customer' does not exist.");
                    return ServicesResult<bool>.Failure("Role 'Customer' does not exist.");
                }

                var result = await _userManager.AddToRoleAsync(user, "Customer");
                if (!result.Succeeded)
                {
                    _logger.LogError("[Service] AddRoleCustomer failed: Unable to assign 'Customer' role to user {UserId}. Errors: {Errors}", user.Id, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return ServicesResult<bool>.Failure("Failed to assign role.");
                }

                _logger.LogInformation("[Service] Successfully assigned 'Customer' role to user {UserId}", user.Id);
                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Service] AddRoleCustomer failed: Unexpected error for user {UserId}", user.Id);
                return ServicesResult<bool>.Failure("An unexpected error occurred.");
            }
        }
        #endregion
    }
}
