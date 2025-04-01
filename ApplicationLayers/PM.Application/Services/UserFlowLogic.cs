using Microsoft.Extensions.Logging;
using PM.Application.Contracts.Interfaces;
using PM.Application.DTOs.Users;
using PM.Application.Features.Users.Commands;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace PM.Application.Services
{
    public class UserFlowLogic : IUserFlowLogic
    {
        private readonly IUserServices _userServices;
        private readonly IActivityLogServices _activityLogServices;
        private readonly ILogger<UserFlowLogic> _logger;

        public UserFlowLogic(IUserServices userServices, IActivityLogServices activityLogServices, ILogger<UserFlowLogic> logger)
        {
            _userServices = userServices;
            _activityLogServices = activityLogServices;
            _logger = logger;
        }

        public async Task<ServicesResult<bool>> PatchUserAsync(PacthUserCommand command)
        {
            var result = await _userServices.PatchUserAsync(command.UserId, new User
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Email,
                PhoneNumber = command.NumberPhone,
                FullName = string.Concat(command.FirstName, " ", command.LastName),
                UserName = command.UserName,
                AvatarPath = command.AvatarUrl
            });

            if (!result.Status)
            {
                _logger.LogError("[PatchUser] Failed to patch user. UserId={UserId}", command.UserId);
                return ServicesResult<bool>.Failure(result.Message);
            }

            await LogUserAction(command.UserId, "Patch User");
            _logger.LogInformation("[PatchUser] User patched successfully. UserId={UserId}", command.UserId);
            return ServicesResult<bool>.Success(true);
        }

        public async Task<ServicesResult<UserDetailDTO>> DetailUser(string userId)
        {
            var detailUser = await _userServices.GetDetailUserAsync(userId);

            if (!detailUser.Status)
            {
                _logger.LogWarning("[DetailUser] User not found. UserId={UserId}", userId);
                return ServicesResult<UserDetailDTO>.Failure(detailUser.Message);
            }
            var response = new UserDetailDTO()
            {
                UserId = userId,
                FullName = detailUser.Data!.FullName,
                Avata =  detailUser.Data.AvatarPath,
                Email = detailUser.Data!.Email   ?? "empty",
                UserName = detailUser.Data!.UserName ?? "emtpy",
                Phone = detailUser.Data!.PhoneNumber ?? "empty"
            };
            _logger.LogInformation("[DetailUser] Retrieved user details successfully. UserId={UserId}", userId);
            return ServicesResult<UserDetailDTO>.Success(response);
        }

        public async Task<ServicesResult<bool>> UpdateUserAsync(PacthUserCommand command)
        {
            var result = await _userServices.UpdateUserAsync(new User
            {
                Id = command.UserId,
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Email,
                PhoneNumber = command.NumberPhone,
                FullName = string.Concat(command.FirstName, " ", command.LastName),
                UserName = command.UserName,
                AvatarPath = command.AvatarUrl
            });

            if (!result.Status)
            {
                _logger.LogError("[UpdateUser] Failed to update user. UserId={UserId}", command.UserId);
                return ServicesResult<bool>.Failure(result.Message);
            }

            await LogUserAction(command.UserId, "Update User");
            _logger.LogInformation("[UpdateUser] User updated successfully. UserId={UserId}", command.UserId);
            return ServicesResult<bool>.Success(true);
        }

        private async Task LogUserAction(string userId, string action)
        {
            var log = new ActivityLog
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Action = action,
                ActionDate = DateTime.UtcNow
            };

            var addLog = await _activityLogServices.AddAsync(log);
            if (!addLog.Status)
            {
                _logger.LogError("[LogUserAction] Failed to store user action. UserId={UserId}, Action={Action}", userId, action);
            }
        }
    }
}
