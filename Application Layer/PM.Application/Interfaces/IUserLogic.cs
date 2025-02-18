using PM.Domain.Models.users;

namespace PM.Application.Interfaces
{
    public interface IUserLogic
    {
        public Task<string> DetailUser(string token);
        //public Task<string> UpdateUser(DetailAppUser user);
        //public Task<string> ChangePassword(string token, string oldPassword, string newPassword);
        //public Task<string> UpdateAvata(string token, string avata);
    }
}
