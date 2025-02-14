using PM.Application.Interfaces;
using PM.Domain.Interfaces.Services;
using PM.Domain.Models.auths;

namespace PM.Application.Implements
{
    public class AuthLogic : IAuthLogic
    {
        private readonly IAuthServices _authServices;
        public AuthLogic(IAuthServices authServices)
        {
            _authServices = authServices;
        }
        public async Task<string> Login(LoginModel loginModel)
        {
            var loginResult = await _authServices.LoginAsync(loginModel);
            if(loginResult.Status == false)
            {
                return loginResult.Message;
            }
            return $"Login success. {loginResult.Data.ToString()}";
        }
        public async Task<string> Register(RegisterModel registerModel)
        {
            var registerResult = await _authServices.RegisterAsync(registerModel);
            if (registerResult.Status == false)
            {
                return registerResult.Message;
            }
            return $"Register success. {registerResult.Data.ToString()}";
        }
        public Task<string> LogOut()
        {
            var logOutResult = _authServices.LogOutAsync();
            if (logOutResult.Result.Status == false)
            {
                return Task.FromResult(logOutResult.Result.Message);
            }
            return Task.FromResult("Log out success");
        }
        public Task<string> ForgotPassword(ForgotPasswordModel model)
        {
            var forgotPasswordResult = _authServices.ForgotPassword(model);
            if (forgotPasswordResult.Result.Status == false)
            {
                return Task.FromResult(forgotPasswordResult.Result.Message);
            }
            return Task.FromResult("Forgot password success");
        }
    }
}
