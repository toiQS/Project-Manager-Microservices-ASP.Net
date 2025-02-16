using Microsoft.AspNetCore.Identity;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;
using PM.Domain.Models.auths;
using PM.Domain.Models.users;

namespace PM.Persistence.Implements.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        //private readonly IUnitOfWork _unitOfWork;   

        //public AuthServices(
        //    UserManager<User> userManager,
        //    SignInManager<User> signInManager, IUnitOfWork unitOfWork)
        //{
        //    _userManager = userManager;
        //    _signInManager = signInManager;
        //    _unitOfWork = unitOfWork;
        //}

        public AuthServices(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ServicesResult<DetailAppUser>> LoginAsync(LoginModel loginModel)
        {
            if (loginModel is null)
                return ServicesResult<DetailAppUser>.Failure("User Name and Password may not be null");

            try
            {
                var isUserValid = await _userManager.FindByEmailAsync(loginModel.Email);
                if (isUserValid == null)
                    return ServicesResult<DetailAppUser>.Failure("Invalid email");

                var isPasswordValid = await _userManager.CheckPasswordAsync(isUserValid, loginModel.Password);
                if (!isPasswordValid)
                    return ServicesResult<DetailAppUser>.Failure("Invalid password");

                await _signInManager.SignInAsync(isUserValid, isPersistent: false);
                var detailAppUser = new DetailAppUser
                {
                    UserId = isUserValid.Id,
                    UserName = isUserValid.UserName ?? string.Empty, // Handle possible null reference
                    Email = isUserValid.Email,
                    FullName = isUserValid.FullName ?? string.Empty, // Handle possible null reference
                    Phone = isUserValid.PhoneNumber ?? string.Empty, // Handle possible null reference
                    Avata = isUserValid.AvatarPath ?? string.Empty // Handle possible null reference
                };
                return ServicesResult<DetailAppUser>.Success(detailAppUser);
            }
            catch (Exception ex)
            {
                // Log exception here
                return ServicesResult<DetailAppUser>.Failure($"An error occurred: {ex.Message}");
            }
        }
        //public async Task<ServicesResult<DetailAppUser>> LoginMethodSecondAsync(LoginModel loginModel)
        //{
        //    if (loginModel is null)
        //        return ServicesResult<DetailAppUser>.Failure("User Name and Password may not be null");

        //    try
        //    {
        //        var isUserValid = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Email", loginModel.Email);
        //        if (isUserValid == null)
        //            return ServicesResult<DetailAppUser>.Failure("Invalid email");
        //        var isPasswordValid = await _userManager.CheckPasswordAsync(isUserValid.Data, loginModel.Password);
        //        if (!isPasswordValid)
        //            return ServicesResult<DetailAppUser>.Failure("Invalid password");
        //        await _signInManager.SignInAsync(isUserValid.Data, isPersistent: false);
        //        var detailAppUser = new DetailAppUser
        //        {
        //            UserId = isUserValid.Data.Id,
        //            UserName = isUserValid.Data.UserName ?? string.Empty, // Handle possible null reference
        //            Email = isUserValid.Data.Email,
        //            FullName = isUserValid.Data.FullName ?? string.Empty, // Handle possible null reference
        //            Phone = isUserValid.Data.PhoneNumber ?? string.Empty, // Handle possible null reference
        //            Avata = isUserValid.Data.AvatarPath ?? string.Empty // Handle possible null reference
        //        };
        //        return ServicesResult<DetailAppUser>.Success(detailAppUser);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log exception here
        //        return ServicesResult<DetailAppUser>.Failure($"An error occurred: {ex.Message}");
        //    }
        //}

        public async Task<ServicesResult<DetailAppUser>> RegisterAsync(RegisterModel registerModel)
        {
            if (registerModel is null)
                return ServicesResult<DetailAppUser>.Failure("Invalid registration data");

            try
            {
                var user = new User
                {
                    UserName = registerModel.UserName,
                    Email = registerModel.Email
                };

                var result = await _userManager.CreateAsync(user, registerModel.Password);
                if (!result.Succeeded)
                    return ServicesResult<DetailAppUser>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
                return ServicesResult<DetailAppUser>.Success(new DetailAppUser
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                });

            }
            catch (Exception ex)
            {
                // Log exception here
                return ServicesResult<DetailAppUser>.Failure($"An error occurred: {ex.StackTrace}");
            }
        }

        public async Task<ServicesResult<bool>> LogOutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                // Log exception here
                return ServicesResult<bool>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServicesResult<DetailAppUser>> ForgotPassword(ForgotPasswordModel model)
        {
            if (model is null)
                return ServicesResult<DetailAppUser>.Failure("Model cannot be null");

            try
            {
                if (model.NewPassword != model.ComfirmPassword)
                    return ServicesResult<DetailAppUser>.Failure("Password and Confirm Password do not match");

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return ServicesResult<DetailAppUser>.Failure("User not found");

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (!result.Succeeded)
                    return ServicesResult<DetailAppUser>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));

                var detailAppUser = new DetailAppUser
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    Phone = user.PhoneNumber,
                    Avata = user.AvatarPath
                };

                return ServicesResult<DetailAppUser>.Success(detailAppUser);
            }
            catch (Exception ex)
            {
                // Log exception here
                return ServicesResult<DetailAppUser>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _userManager?.Dispose();
        }
    }
}
