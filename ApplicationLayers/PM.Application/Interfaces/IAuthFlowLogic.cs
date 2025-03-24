using Microsoft.AspNetCore.Mvc;
using PM.Application.Features.Auth.Commands;

namespace PM.Application.Interfaces
{
    public interface IAuthFlowLogic
    {
        public Task<IActionResult> Login(LoginCommand loginCommand);

        public Task<IActionResult> Register(RegisterCommand command);
    }
}
