using PM.Shared.Dtos;

namespace PM.Identity.Domain.Interfaces
{
    public interface IAuthServices
    {
        public Task<ServiceResult<string>> Login(string email, string password);
        public Task<ServiceResult<bool>> Logout();
        public Task<ServiceResult<bool>> Register(string email, string username, string password);
        public Task<ServiceResult<bool>> ChangePassword(string email, string oldPassword, string newPassword);
    }
}
