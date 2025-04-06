using PM.Shared.Dtos;

namespace PM.Identity.Application.Interfaces.Services
{
    public interface IAuthService
    {
        public Task<ServiceResult<bool>> Login(string email, string password);
        public Task<ServiceResult<bool>> Logout();
        public Task<ServiceResult<bool>> Register(string email, string username, string password);
        public Task<ServiceResult<bool>> ChangePassword(string email, string oldPassword, string newPassword);
    }
}
