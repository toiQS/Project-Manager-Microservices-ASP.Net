using PM.Domain;
using PM.Domain.Models.users;

namespace PM.Application.Interfaces
{
    public interface IUserLogic
    {
        ServicesResult<DetailAppUser> GetDetailUserToken(string token);
        Task<ServicesResult<DetailAppUser>> GetDetailUserIdentty(string userId);
        Task<ServicesResult<DetailAppUser>> UpdateUser(string token, UpdateAppUser user);
        //public Task<string> ChangePassword(string token, string oldPassword, string newPassword);
        //public Task<string> UpdateAvata(string token, string avata);
    }
}
