using PM.Shared.Dtos.auths;
using System.Data;

namespace PM.Identity.Application.Interfaces
{
    public interface IAuthHandle
    {
        public Task<ServiceResult<string>> RegisterHandleAsync(RegisterModel model);
        public Task<ServiceResult<string>> LoginHandleAsync(LoginModel model);
    }
}
