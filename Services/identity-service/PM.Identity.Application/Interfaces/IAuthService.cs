using PM.Shared.Dtos;

namespace PM.Identity.Application.Interfaces
{
    public interface IAuthService
    {
        public  Task<ServiceResult<bool>> SignInAsync(string email, string password);
        public  Task<ServiceResult<bool>> SignOutAsync();
        public Task<ServiceResult<bool>> RegisterUserAsync(string email, string username, string password);
        public Task<ServiceResult<bool>> ChangeUserPasswordAsync(string email, string oldPassword, string newPassword);

    }
}
