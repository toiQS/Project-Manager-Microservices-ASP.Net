using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    public interface IUserServices
    {
        public Task<ServicesResult<User>> GetDetailUser(string userId);
        public Task<ServicesResult<bool>> UpdateUser(User user);
        public Task<ServicesResult<bool>> PacthUser(string userId, User user);
    }
}
