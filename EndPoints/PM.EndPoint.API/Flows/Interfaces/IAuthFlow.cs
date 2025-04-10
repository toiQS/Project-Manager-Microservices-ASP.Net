using PM.Shared.Dtos.Auths;

namespace PM.EndPoint.API.Flows.Interfaces
{
    public interface IAuthFlow
    {
        public Task<string> HandleLogin(LoginModel loginModel);
        //public Task<string> HandleRegister(RegisterModel registerModel);
        public Task<string> HandleDemo();
    }
}
