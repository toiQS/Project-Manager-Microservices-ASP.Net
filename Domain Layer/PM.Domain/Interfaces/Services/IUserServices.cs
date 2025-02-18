using PM.Domain.Models.users;

namespace PM.Domain.Interfaces.Services
{
    public interface IUserServices
    {
        public Task<ServicesResult<DetailAppUser>> GetDetailUser(string userId);
        public Task<ServicesResult<DetailAppUser>> UpdateUser(string userId, UpdateAppUser user);
        //public Task<ServicesResult<bool>> DeleteUser(string userId);
        //public Task<ServicesResult<bool>> ChangePassword(ChangePasswordUser changePassword);
        //public Task<ServicesResult<bool>> UpdateAvata(string userId, string avata);
    }
}
