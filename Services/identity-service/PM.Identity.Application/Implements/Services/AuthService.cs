using Microsoft.AspNetCore.Identity;
using PM.Identity.Application.Interfaces.Services;
using PM.Identity.Domain.Entities;
using PM.Shared.Dtos;

namespace PM.Identity.Application.Implements.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public async Task<ServiceResult<bool>> SignInAsync(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return ServiceResult<bool>.Failure("User not found.");
                var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                    return ServiceResult<bool>.Success(true);
                else
                    return ServiceResult<bool>.Failure("Invalid login attempt.");   
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure(ex);
            }
        }
        public async Task<ServiceResult<bool>> SignOutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure(ex);
            }
        }
        public async Task<ServiceResult<bool>> RegisterUserAsync(string email, string username, string password)
        {
            try
            {
                var isUserExist = await _userManager.FindByEmailAsync(email);
                if (isUserExist != null)
                    return ServiceResult<bool>.Failure("User already exists.");
                var user = new User
                {
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true
                };
                var addUser = await _userManager.CreateAsync(user, password);
                if (addUser.Succeeded)
                {
                    var role = await _roleManager.FindByNameAsync("Customer");
                    if (role != null)
                    {
                        await _userManager.AddToRoleAsync(user, role.Name!);
                    }
                    return ServiceResult<bool>.Success(true);
                }
                else
                {
                    return ServiceResult<bool>.Failure("User registration failed.");
                }
               
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure(ex);
            }
        }
        public async Task<ServiceResult<bool>> ChangeUserPasswordAsync(string email, string oldPassword, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if(user== null)
                    return ServiceResult<bool>.Failure("User not found.");
                var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
                if (result.Succeeded)
                    return ServiceResult<bool>.Success(true);
                else
                    return ServiceResult<bool>.Failure("Password change failed.");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure(ex);
            }
        }
    }
}
