using Microsoft.AspNetCore.Mvc;
using PM.Application.Features.Auth.Commands;
using PM.Domain;

namespace PM.Application.Interfaces
{
    public interface IAuthFlowLogic
    {
        public Task<ServicesResult<string>> Login(LoginCommand loginCommand);

        public Task<ServicesResult<bool>> Register(RegisterCommand command);
    }
}
