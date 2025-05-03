using PM.Shared.Dtos.users;
using PM.Shared.Dtos.auths;

namespace PM.Identity.Application.Interfaces
{
    public interface IUserHandle
    {
        Task<ServiceResult<UserDetail>> GetUser(string userId);
        Task<ServiceResult<UserDetail>> PatchUserHandle(string userId, UserPatchModel model);
    }
}
