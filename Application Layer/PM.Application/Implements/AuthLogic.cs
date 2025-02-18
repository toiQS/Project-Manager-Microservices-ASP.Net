using PM.Application.Interfaces;
using PM.Domain.Interfaces.Services;
using PM.Domain.Models.auths;
using PM.Infrastructers.Interfaces;

namespace PM.Application.Implements
{
    public class AuthLogic : IAuthLogic
    {
        private readonly IAuthServices _authServices;
        private readonly IJwtServices _jwtServices;
        public AuthLogic(IAuthServices authServices, IJwtServices jwtServices)
        {
            _authServices = authServices;
            _jwtServices = jwtServices;
        }
        public async Task<string> Login(LoginModel loginModel)
        {
            var loginResult = await _authServices.LoginAsync(loginModel);
            if(loginResult.Status == false  || loginResult.Data == null)
            {
                return loginResult.Message;
            }
            var token = _jwtServices.GenerateToken(loginResult.Data);
            if(token.Status == false)
            {
                return token.Message + " token";
            }
            return $"Login success. \n token: \n{token.Data}";
        }
        //public async Task<string> LoginMethodSecond(LoginModel loginModel)
        //{
        //    var loginResult = await _authServices.LoginMethodSecondAsync(loginModel);
        //    if (loginResult.Status == false)
        //    {
        //        return loginResult.Message;
        //    }
        //    return $"Login success. {loginResult.Data.ToString()}";
        //}
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
