using PM.Shared.Dtos.Auths;
using PM.Shared.Dtos;

namespace PM.Identity.Application.Implements.Flows
{
    public interface IAuthFlow
    {
        public Task<ServiceResult<string>> Login(LoginModel loginModel);
    }
}
