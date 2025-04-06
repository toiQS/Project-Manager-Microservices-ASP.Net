using Microsoft.AspNetCore.Identity;
using PM.Identity.Application.Interfaces.Services;
using PM.Identity.Domain.Entities;
using PM.Identity.Infrastructure.Data;
using PM.Shared.Dtos;
using PM.Shared.Persistence.Interfaces;

namespace PM.Identity.Application.Implements.Services
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
                if(user == null)
                    return ServiceResult<User>.Failure("User not found.");
                return ServiceResult<User>.Success(user);
            }
            catch (Exception ex)
            {
                return ServiceResult<User>.Failure(ex);
            }
        }
    }
}
