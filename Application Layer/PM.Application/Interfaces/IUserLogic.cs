using Microsoft.AspNetCore.Mvc;
using PM.Application.Models.users;
using PM.Domain.Models.users;

namespace PM.Application.Interfaces
{
    public interface IUserLogic
    {
        public IActionResult GetDetailUserToken(string token);
        public Task<IActionResult> GetDetailUserIdentity(string userId);
        public Task<IActionResult> UpdateUser(UpdateUserModel model);
        public Task<IActionResult> ChangePassword(ChangePasswordModel model);
        public Task<IActionResult> UpdateAvatar(UpdateAvataModel model);
    }
}
