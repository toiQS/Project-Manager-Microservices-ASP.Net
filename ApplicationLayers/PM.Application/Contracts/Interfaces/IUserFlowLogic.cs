using Microsoft.AspNetCore.Mvc;
using PM.Application.DTOs.Users;
using PM.Application.Features.Users.Commands;
using PM.Domain;

namespace PM.Application.Contracts.Interfaces
{
    public interface IUserFlowLogic
    {
        public Task<ServicesResult<bool>> PatchUserAsync(PacthUserCommand command);
        public Task<ServicesResult<UserDetailDTO>> DetailUser(string userId);
        public Task<ServicesResult<bool>> UpdateUserAsync(PacthUserCommand command);
    }
}
