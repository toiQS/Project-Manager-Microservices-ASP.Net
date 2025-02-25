using Microsoft.AspNetCore.Identity;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;
using PM.Domain.Models.users;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        #region GetDetailUser
        /// <summary>
        /// Retrieves detailed information about a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>Returns user details if found, otherwise returns an error message.</returns>
        public async Task<ServicesResult<DetailAppUser>> GetDetailUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return ServicesResult<DetailAppUser>.Failure("User not found");
            }
            try
            {
                var userResponse = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);
                if (!userResponse.Status)
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
                if (!roles.Any()) return ServicesResult<DetailAppUser>.Failure("Role not found");

                response.Role = roles.FirstOrDefault();
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
        #endregion

        #region UpdateUser
        /// <summary>
        /// Updates the user information.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="user">The updated user data.</param>
        /// <returns>Returns the updated user details if successful, otherwise an error message.</returns>
        public async Task<ServicesResult<DetailAppUser>> UpdateUser(string userId, UpdateAppUser user)
        {
            if (user is null)
            {
                return ServicesResult<DetailAppUser>.Failure("User not found");
            }
            try
            {
                var userResponse = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);
                if (!userResponse.Status)
                {
                    return ServicesResult<DetailAppUser>.Failure(userResponse.Message);
                }

                // Update user fields
                userResponse.Data.FirstName = user.FirstName ?? userResponse.Data.FirstName;
                userResponse.Data.LastName = user.LastName ?? userResponse.Data.LastName;
                userResponse.Data.FullName = string.IsNullOrWhiteSpace(user.FirstName) || string.IsNullOrWhiteSpace(user.LastName) ? userResponse.Data.FullName : $"{user.FirstName} {user.LastName}";
                userResponse.Data.Email = user.Email ?? userResponse.Data.Email;
                userResponse.Data.PhoneNumber = user.Phone ?? userResponse.Data.PhoneNumber;
                userResponse.Data.AvatarPath = user.PathImage ?? userResponse.Data.AvatarPath;
                userResponse.Data.UserName = user.UserName ?? userResponse.Data.UserName;

                var updateResponse = await _unitOfWork.UserRepository.UpdateAsync(userResponse.Data);
                if (!updateResponse.Status)
                {
                    return ServicesResult<DetailAppUser>.Failure(updateResponse.Message);
                }

                return await GetDetailUser(userId);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailAppUser>.Failure(ex.Message);
            }
        }
        #endregion

        #region ChangePassword
        /// <summary>
        /// Changes the password of a user.
        /// </summary>
        /// <param name="user">User credentials containing email, old and new passwords.</param>
        /// <returns>Success or failure message.</returns>
        public async Task<ServicesResult<string>> ChangePassword(ChangePasswordUser user)
        {
            if (user is null)
            {
                return ServicesResult<string>.Failure("User not found");
            }
            try
            {
                var userResponse = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Email", user.Email);
                if (!userResponse.Status)
                {
                    return ServicesResult<string>.Failure(userResponse.Message);
                }

                var result = await _userManager.ChangePasswordAsync(userResponse.Data, user.OldPassword, user.NewPassword);
                if (!result.Succeeded)
                {
                    return ServicesResult<string>.Failure(result.Errors.FirstOrDefault()?.Description);
                }

                return ServicesResult<string>.Success("Success");
            }
            catch (Exception ex)
            {
                return ServicesResult<string>.Failure(ex.Message);
            }
        }
        #endregion

        #region UpdateAvata
        /// <summary>
        /// Updates the avatar of a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="avata">The new avatar URL/path.</param>
        /// <returns>Returns the updated user details if successful, otherwise an error message.</returns>
        public async Task<ServicesResult<DetailAppUser>> UpdateAvata(string userId, string avata)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return ServicesResult<DetailAppUser>.Failure("User not found");
            }
            try
            {
                var userResponse = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);
                if (!userResponse.Status)
                {
                    return ServicesResult<DetailAppUser>.Failure(userResponse.Message);
                }

                userResponse.Data.AvatarPath = avata;
                var updateResponse = await _unitOfWork.UserRepository.UpdateAsync(userResponse.Data);
                if (!updateResponse.Status)
                {
                    return ServicesResult<DetailAppUser>.Failure(updateResponse.Message);
                }

                return await GetDetailUser(userId);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailAppUser>.Failure(ex.Message);
            }
        }
        #endregion
    }
}