using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    public interface IAuthServices
    {
        public Task<ServicesResult<bool>> Login(string email, string password);
        public Task<ServicesResult<bool>> Register (string email, string username, string password);
        public Task<ServicesResult<User>> GetUserByEmail(string email);
        public Task<ServicesResult<bool>> AddRoleCustomer(User user);
    }
}
