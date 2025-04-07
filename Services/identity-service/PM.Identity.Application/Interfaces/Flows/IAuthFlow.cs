using PM.Shared.Dtos.Auths;
using PM.Shared.Dtos;
using System.Threading.Tasks;

namespace PM.Identity.Application.Interfaces.Flows
{
    public interface IAuthFlow
    {
        public Task<ServiceResult<string>> HandleSignInAsync(LoginModel loginModel);
        public Task<ServiceResult<string>> HandleRegisterUserAsync(RegisterModel model);
        public Task<ServiceResult<string>> HandleSignOutAsync(string token);
        public Task<ServiceResult<string>> HandleChangePasswordAsync(ChangePassword changePassword);
    }
}
