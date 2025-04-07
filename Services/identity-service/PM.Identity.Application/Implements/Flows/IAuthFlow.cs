using PM.Shared.Dtos.Auths;
using PM.Shared.Dtos;
using System.Threading.Tasks;

namespace PM.Identity.Application.Implements.Flows
{
    public interface IAuthFlow
    {
        public Task<ServiceResult<string>> Login(LoginModel loginModel);
        public Task<ServiceResult<string>> Register(RegisterModel model);
        public Task<ServiceResult<string>> LogOut(string token);
        public Task<ServiceResult<string>> ChangePassword(string email, string oldPassword, string newPassword);
    }
}
