using PM.Application.Interfaces;
using PM.Domain.Interfaces.Services;
using PM.Domain.Models.users;
using PM.Infrastructers.Interfaces;

namespace PM.Application.Implements
{
    public class UserLogic : IUserLogic
    {
        private readonly IUserServices _userServices;
        private readonly IJwtServices _jwtServices;
        public UserLogic(IUserServices userServices, IJwtServices jwtServices)
        {
            _userServices = userServices;
            _jwtServices = jwtServices;
        }
        public async Task<string> DetailUser(string token)
        {
            var response =  _jwtServices.ParseToken(token);
            if(response.Status == false)
            {
                return response.Message;
            }
            return $"Id: {response.Data.UserId} - User: {response.Data.UserName} - {response.Data.FullName} - {response.Data.Email} - {response.Data.Phone} - {response.Data.Avata}";
        }
        //public Task<string> UpdateUser(DetailAppUser user);
        //public Task<string> ChangePassword(string token, string oldPassword, string newPassword);
        //public Task<string> UpdateAvata(string token, string avata);
    }
}
