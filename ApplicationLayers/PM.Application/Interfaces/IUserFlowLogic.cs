using Microsoft.AspNetCore.Mvc;
using PM.Application.Features.Users.Commands;

namespace PM.Application.Interfaces
{
    public interface IUserFlowLogic
    {
        public Task<IActionResult> PatchUserAsync(PacthUserCommand command);
        public Task<IActionResult> DetailUser(string userId);
        public Task<IActionResult> UpdateUserAsync(PacthUserCommand command);
    }
}
