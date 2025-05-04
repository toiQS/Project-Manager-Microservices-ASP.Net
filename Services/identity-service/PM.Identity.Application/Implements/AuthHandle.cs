using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using PM.Identity.Application.Interfaces;
using PM.Identity.Entities;
using PM.Shared.Dtos;
using PM.Shared.Dtos.auths;
using PM.Shared.Jwt;

namespace PM.Identity.Application.Implements
{
    public class AuthHandle : IAuthHandle
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IPasswordHasher<AppUser> _passwordHasher;
        private readonly IPasswordValidator<AppUser> _passwordValidator;
        private readonly IJwtService _jwtService;
        public AuthHandle(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IPasswordHasher<AppUser> passwordHasher,
            IPasswordValidator<AppUser> passwordValidator,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _passwordHasher = passwordHasher;
            _passwordValidator = passwordValidator;
            _jwtService = jwtService;
        }
        public async Task<ServiceResult<string>> RegisterHandleAsync(RegisterModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return ServiceResult<string>.Error("Username, email and password are required.");
            }
            try
            {
                var user = new AppUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    EmailConfirmed = false
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return ServiceResult<string>.Success(user.Id);
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return ServiceResult<string>.Error(errors);
                }

            }
            catch (Exception ex)
            {
                return ServiceResult<string>.FromException(ex);
            }
        }
        public async Task<ServiceResult<string>> LoginHandleAsync(LoginModel model)
        {
            if(string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return ServiceResult<string>.Error("Email and password are required.");
            }
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return ServiceResult<string>.Error("Invalid email or password.");
                }
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    //var role = await _userManager.GetRolesAsync(user);
                    var token = _jwtService.GenerateToken(user.Id, user.Email!, "customer");
                    if(token.Status != ResultStatus.Success)
                    {
                        return ServiceResult<string>.Error(token.Message);
                    }
                    
                    return ServiceResult<string>.Success(token.Data!);
                }
                else
                {
                    return ServiceResult<string>.Error("Invalid email or password.");
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.FromException(ex);
            }
        }
        public async Task<ServiceResult<string>> ForgotPasswordHandle(ForgotPasswordModel model)
        {
            if(string.IsNullOrEmpty(model.Email))
            {
                return ServiceResult<string>.Error("Email is required.");
            }
            if(string.IsNullOrEmpty(model.NewPassword))
            {
                return ServiceResult<string>.Error("New password is required.");
            }
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return ServiceResult<string>.Error("Invalid email.");
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (result.Succeeded)
                {
                    return ServiceResult<string>.Success(user.Id);
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return ServiceResult<string>.Error(errors);
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.FromException(ex);
            }
        }
        public async Task<ServiceResult<string>> ChangePasswordHandle(ChangePasswordModel model)
        {
            if(string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.OldPassword) || string.IsNullOrEmpty(model.NewPassword))
            {
                return ServiceResult<string>.Error("Email, old password and new password are required.");
            }
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if(user == null)
                {
                    return ServiceResult<string>.Error("Invalid email.");
                }
                var result = await _signInManager.CheckPasswordSignInAsync(user, model.OldPassword, false);
                if (!result.Succeeded)
                {
                    return ServiceResult<string>.Error("Invalid old password.");
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (resetResult.Succeeded)
                {
                    return ServiceResult<string>.Success(user.Id);
                }
                else
                {
                    var errors = string.Join(", ", resetResult.Errors.Select(e => e.Description));
                    return ServiceResult<string>.Error(errors);
                }
            }
            catch(Exception ex)
            {
                return ServiceResult<string>.FromException(ex);
            }
        }
    }
}
