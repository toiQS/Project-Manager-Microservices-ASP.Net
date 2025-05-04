using Microsoft.AspNetCore.Identity;
using PM.Identity.Application.Interfaces;
using PM.Identity.Entities;
using PM.Identity.Infrastructure.Data;
using PM.Shared.Dtos;
using PM.Shared.Dtos.users;
using PM.Shared.Handle.Interfaces;

namespace PM.Identity.Application.Implements
{
    public class UserHandle : IUserHandle
    {
        private readonly IUnitOfWork<AuthDbContext> _unitOfWork;
        private readonly UserManager<AppUser> _userManager; 
        
        public UserHandle(IUnitOfWork<AuthDbContext> unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<ServiceResult<UserDetail>> GetUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<UserDetail>.Error("User ID cannot be null or empty.");
            }
            try
            {
                var userRepo = _unitOfWork.Repository<AppUser>();
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ServiceResult<UserDetail>.Error("User not found.");
                }
                var roles = await _userManager.GetRolesAsync(user);

                var userDetail = new UserDetail
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? "Empty",
                    UserName = user.UserName ?? "Empty",
                    AvataPath = user.Avatar ?? "Empty",
                    PhoneNumber = user.PhoneNumber ?? "Empty",
                    Role = roles.FirstOrDefault() ?? "Customer"
                };
                return ServiceResult<UserDetail>.Success(userDetail);
            }
            catch (Exception ex)
            {
                return ServiceResult<UserDetail>.FromException(ex);
            }
        }
        public async Task<ServiceResult<UserDetail>> PatchUserHandle(string userId, UserPatchModel model)
        {
            if(userId == null)
            {
                return ServiceResult<UserDetail>.Error("User ID cannot be null or empty.");
            }
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>
            {
                {nameof(model.FirstName), model.FirstName},
                {nameof(model.LastName), model.LastName},
                {nameof(model.Email), model.Email},
                {nameof(model.PhoneNumber), model.PhoneNumber},
                {"Avatar", model.AvataPath},
                {"FullName", $"{model.FirstName} {model.LastName}"}
            };
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if(user == null)
                {
                    return ServiceResult<UserDetail>.Error("User not found.");
                }
                var resultAction = await _unitOfWork.ExecuteTransactionAsync(async () => await _unitOfWork.Repository<AppUser>().PatchAsync(user, keyValuePairs), CancellationToken.None);
                if(resultAction.Status != ResultStatus.Success)
                {
                    return ServiceResult<UserDetail>.Error("Update user failed.");
                }
                return await GetUser(userId);
            }
            catch (Exception ex)
            {
                return ServiceResult<UserDetail>.FromException(ex);
            }
        }
        
    }
}
