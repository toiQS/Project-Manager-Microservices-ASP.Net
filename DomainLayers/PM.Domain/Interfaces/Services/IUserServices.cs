using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    public interface IUserServices
    {
        public Task<ServicesResult<User>> GetDetailUserAsync(string userId);
        public Task<ServicesResult<bool>> UpdateUserAsync(User user);
        public Task<ServicesResult<bool>> PatchUserAsync(string userId, User user);
    }
}
