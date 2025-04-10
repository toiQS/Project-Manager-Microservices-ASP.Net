using Microsoft.AspNetCore.Identity;
using PM.Identity.Application.Interfaces;
using PM.Identity.Domain.Entities;
using PM.Identity.Infrastructure.Data;
using PM.Shared.Dtos;
using PM.Shared.Persistence.Interfaces;

namespace PM.Identity.Application.Implements
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork<AuthDbContext> _unitOfWork;
        private readonly UserManager<User> _userManager;
        public UserService(IUnitOfWork<AuthDbContext> unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<ServiceResult<User>> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return ServiceResult<User>.Failure("User not found.");
                return ServiceResult<User>.Success(user);
            }
            catch (Exception ex)
            {
                return ServiceResult<User>.Failure(ex);
            }
        }
        public async Task<ServiceResult<User>> GetUserById(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return ServiceResult<User>.Failure("User not found.");
                return ServiceResult<User>.Success(user);
            }
            catch (Exception ex)
            {
                return ServiceResult<User>.Failure(ex);
            }
        }
        public async Task<ServiceResult<bool>> UpdateUser(string id, string firstName, string lastName, string username, string avatarPath)
        {
            var user = await _unitOfWork.UserRepository.GetOneAsync("Id", id);
            if (user == null)
                return ServiceResult<bool>.Failure("User not found.");
            Dictionary<string, object> updateData = new Dictionary<string, object>()
            {
                {
                    "FirstName", firstName
                },
                {
                    "LastName", lastName
                },
                {
                    "UserName", username
                },
                {
                    "AvatarPath", avatarPath
                },
                {
                    "FullName", $"{firstName} {lastName}"
                }
            };
            var updateResult = await _unitOfWork.ExecuteTransactionAsync(async() =>
            {
                await _unitOfWork.UserRepository.PacthAsync(user.Data!, updateData);
            });
            if (updateResult.Status)
            {
                return ServiceResult<bool>.Success(true);
            }
            else
            {
                return ServiceResult<bool>.Failure("Failed to update user.");
            }
        }
    }
}
