using PM.Domain;
using PM.Domain.Models.auths;

namespace PM.Application.Interfaces
{
    public interface IAuthLogic
    {
        public Task<string> Login(LoginModel loginModel);
        //public Task<string> LoginMethodSecond(LoginModel loginModel);
        public Task<string> Register(RegisterModel registerModel);
        public Task<string> LogOut(string token);
        public Task<string> ForgotPassword(ForgotPasswordModel model);
    }
}
