using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PM.Application.Features.Users.Commands;
using PM.Application.Interfaces;
using PM.Domain.Entities;
using PM.Domain.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace PM.Application.Services
{
    public class UserFlowLogic : ControllerBase, IUserFlowLogic
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

        public async Task<IActionResult> PatchUserAsync(PacthUserCommand command)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("[PatchUser] Invalid model state.");
                return BadRequest("Invalid request data.");
            }

            var user = new User
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Email,
                PhoneNumber = command.NumberPhone,
                FullName = string.Concat(command.FirstName, " ", command.LastName),
                UserName = command.UserName,
                AvatarPath = command.AvatarUrl
            };

            var patchUser = await _userServices.PatchUserAsync(command.UserId, user);
            if (!patchUser.Status)
            {
                _logger.LogError("[PatchUser] Failed to patch user. UserId={UserId}", command.UserId);
                return BadRequest(patchUser.Message);
            }

            await LogUserAction(command.UserId, "Patch User");

            var detailUser = await _userServices.GetDetailUserAsync(command.UserId);
            if (!detailUser.Status)
            {
                _logger.LogError("[PatchUser] Failed to retrieve user details. UserId={UserId}", command.UserId);
                return BadRequest(detailUser.Message);
            }

            _logger.LogInformation("[PatchUser] User patched successfully. UserId={UserId}", command.UserId);
            return Ok(detailUser.Data);
        }

        public async Task<IActionResult> DetailUser(string userId)
        {
            var detailUser = await _userServices.GetDetailUserAsync(userId);

            if (!detailUser.Status)
            {
                _logger.LogWarning("[DetailUser] User not found. UserId={UserId}", userId);
                return BadRequest(detailUser.Message);
            }

            _logger.LogInformation("[DetailUser] Retrieved user details successfully. UserId={UserId}", userId);
            return Ok(detailUser.Data);
        }

        public async Task<IActionResult> UpdateUserAsync(PacthUserCommand command)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("[UpdateUser] Invalid model state.");
                return BadRequest("Invalid request data.");
            }

            var user = new User
            {
                Id = command.UserId,
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Email,
                PhoneNumber = command.NumberPhone,
                FullName = string.Concat(command.FirstName, " ", command.LastName),
                UserName = command.UserName,
                AvatarPath = command.AvatarUrl
            };

            var updateUser = await _userServices.UpdateUserAsync(user);
            if (!updateUser.Status)
            {
                _logger.LogError("[UpdateUser] Failed to update user. UserId={UserId}", command.UserId);
                return BadRequest(updateUser.Message);
            }

            await LogUserAction(command.UserId, "Update User");

            var detailUser = await _userServices.GetDetailUserAsync(command.UserId);
            if (!detailUser.Status)
            {
                _logger.LogError("[UpdateUser] Failed to retrieve updated user details. UserId={UserId}", command.UserId);
                return BadRequest(detailUser.Message);
            }

            _logger.LogInformation("[UpdateUser] User updated successfully. UserId={UserId}", command.UserId);
            return Ok(detailUser.Data);
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
