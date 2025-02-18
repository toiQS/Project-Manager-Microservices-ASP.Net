using Microsoft.AspNetCore.Identity;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;
using PM.Domain.Models.users;
using System.Globalization;

namespace PM.Persistence.Implements.Services
{
    public class UserServices : IUserServices
    {
        #region Constructor
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        public UserServices(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        #endregion
        public async Task<ServicesResult<DetailAppUser>> GetDetailUser(string userId)
        {
            if(string.IsNullOrEmpty(userId))
            {
                return ServicesResult<DetailAppUser>.Failure("User not found");
            }
            try
            {
                var userResponse = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id",userId);
                if (userResponse.Status == false)
                {
                    return ServicesResult<DetailAppUser>.Failure(userResponse.Message);
                }
                var response = new DetailAppUser()
                {
                    UserId = userResponse.Data.Id,
                    UserName = userResponse.Data.UserName,
                    FullName = userResponse.Data.FullName,
                    Avata = userResponse.Data.AvatarPath,
                    Email = userResponse.Data.Email,
                    Phone = userResponse.Data.PhoneNumber,
                };
                var roles = await _userManager.GetRolesAsync(userResponse.Data);
                if(!roles.Any()) return ServicesResult<DetailAppUser>.Failure("Role not found");
                response.Role = roles[0];
                return ServicesResult<DetailAppUser>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailAppUser>.Failure(ex.Message);
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        public async Task<ServicesResult<DetailAppUser>> UpdateUser(string userId,UpdateAppUser user)
        {
            if(user is null)
            {
                return ServicesResult<DetailAppUser>.Failure("User not found");
            }
            try
            {
                var userResponse = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);
                if(userResponse.Status == false)
                {
                    return ServicesResult<DetailAppUser>.Failure(userResponse.Message);
                }
                userResponse.Data.FirstName = user.FirstName ?? userResponse.Data.FirstName;
                userResponse.Data.LastName = user.LastName ?? userResponse.Data.LastName;
                userResponse.Data.FullName = $"{user.FirstName} {user.LastName}" ?? userResponse.Data.FullName;
                userResponse.Data.Email = user.Email ?? userResponse.Data.Email;
                userResponse.Data.PhoneNumber = user.Phone ?? userResponse.Data.PhoneNumber;
                userResponse.Data.AvatarPath = user.PathImage ?? userResponse.Data.AvatarPath;
                userResponse.Data.UserName = user.UserName ?? userResponse.Data.UserName;

                var updateResponse = await _unitOfWork.UserRepository.UpdateAsync(userResponse.Data);
                if(updateResponse.Status == false)
                {
                    return ServicesResult<DetailAppUser>.Failure(updateResponse.Message);
                }
                var response = await GetDetailUser(userId);
                if (response.Status == false)
                {
                    return ServicesResult<DetailAppUser>.Failure(response.Message);
                }
                return ServicesResult<DetailAppUser>.Success(response.Data);

            }
            catch(Exception ex)
            {
                return ServicesResult<DetailAppUser>.Failure(ex.Message);
            }
            //finally
            //{
            //    await _unitOfWork.SaveChangesAsync();
            //    _unitOfWork.Dispose();
            //}
        }

        public async Task<ServicesResult<string>> ChangePassword( ChangePasswordUser user)
        {
            if (user is null)
            {
                return ServicesResult<string>.Failure("User not found");
            }
            try
            {
                var userResponse = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Email", user.Email);
                if (userResponse.Status == false)
                {
                    return ServicesResult<string>.Failure(userResponse.Message);
                }
                var result = await _userManager.ChangePasswordAsync(userResponse.Data, user.OldPassword, user.NewPassword);
                if (!result.Succeeded)
                {
                    return ServicesResult<string>.Failure(result.Errors.FirstOrDefault().Description);
                }
                return ServicesResult<string>.Success("Success");
            }
            catch (Exception ex)
            {
                return ServicesResult<string>.Failure(ex.Message);
            }
            //finally
            //{
            //    await _unitOfWork.SaveChangesAsync();
            //    _unitOfWork.Dispose();
            //}
        }
        public async Task<ServicesResult<DetailAppUser>> UpdateAvata(string userId, string avata)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return ServicesResult<DetailAppUser>.Failure("User not found");
            }
            try
            {
                var userResponse = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);
                if (userResponse.Status == false)
                {
                    return ServicesResult<DetailAppUser>.Failure(userResponse.Message);
                }
                userResponse.Data.AvatarPath = avata;
                var updateResponse = await _unitOfWork.UserRepository.UpdateAsync(userResponse.Data);
                if (updateResponse.Status == false)
                {
                    return ServicesResult<DetailAppUser>.Failure(updateResponse.Message);
                }
                var response = await GetDetailUser(userId);
                if (response.Status == false)
                {
                    return ServicesResult<DetailAppUser>.Failure(response.Message);
                }
                return ServicesResult<DetailAppUser>.Success(response.Data);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailAppUser>.Failure(ex.Message);
            }
            //finally
            //{
            //    await _unitOfWork.SaveChangesAsync();
            //    _unitOfWork.Dispose();
            //}
        }
    }
}
