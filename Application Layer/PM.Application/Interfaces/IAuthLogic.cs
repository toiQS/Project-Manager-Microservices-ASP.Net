using Microsoft.AspNetCore.Mvc;
using PM.Domain.Models.auths;

namespace PM.Application.Interfaces
{
    public interface IAuthLogic
    {
        public Task<IActionResult> Login(LoginModel loginModel);
        public Task<IActionResult> Register(RegisterModel registerModel);
        public Task<IActionResult> LogOut(string token);
        public Task<IActionResult> ForgotPassword(ForgotPasswordModel model);
        
    }
}
