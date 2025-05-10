using PM.Identity.Application.Dtos.auths;
using PM.Shared.Dtos.Models;

namespace PM.Identity.Application.Interfaces
{
    public interface IAuthHandle
    {
        Task<ServiceResult<string>> RegisterHandleAsync(RegisterModel model);
        Task<ServiceResult<string>> LoginHandleAsync(LoginModel model);
        Task<ServiceResult<string>> ChangePasswordHandle(ChangePasswordModel model);
        Task<ServiceResult<string>> ForgotPasswordHandle(ForgotPasswordModel model);    
    }
}
