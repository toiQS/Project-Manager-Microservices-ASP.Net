using PM.Domain.Models.users;

namespace PM.Domain.Interfaces.Services
{
    interface IUserServices
    {
        public Task<ServicesResult<DetailAppUser>> GetUserById(string userId);
        public Task<ServicesResult<DetailAppUser>> UpdateUser(DetailAppUser user);
        public Task<ServicesResult<bool>> DeleteUser(string userId);
        public Task<ServicesResult<bool>> ChangePassword(ChangePasswordUser changePassword);    
    }
}
