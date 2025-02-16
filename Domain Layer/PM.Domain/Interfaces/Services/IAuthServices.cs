using PM.Domain.Models.auths;
using PM.Domain.Models.users;

namespace PM.Domain.Interfaces.Services
{
    public interface IAuthServices : IDisposable
    {
        public Task<ServicesResult<DetailAppUser>> LoginAsync(LoginModel model);
        //public Task<ServicesResult<DetailAppUser>> LoginMethodSecondAsync(LoginModel model);
        public Task<ServicesResult<DetailAppUser>> RegisterAsync(RegisterModel model);
        public Task<ServicesResult<bool>> LogOutAsync();
        public Task<ServicesResult<DetailAppUser>> ForgotPassword(ForgotPasswordModel model);
    }
}
