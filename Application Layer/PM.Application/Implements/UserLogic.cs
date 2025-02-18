using PM.Application.Interfaces;
using PM.Domain;
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
        public ServicesResult<DetailAppUser> GetDetailUserToken(string token)
        {
            var response = _jwtServices.ParseToken(token);
            if (response.Status == false)
            {
                return ServicesResult<DetailAppUser>.Failure(response.Message);
            }
            return ServicesResult<DetailAppUser>.Success(response.Data);
        }
        public async Task<ServicesResult<DetailAppUser>> GetDetailUserIdentty(string userId)
        {
            var response = await _userServices.GetDetailUser(userId);
            if(response.Status == false)
            {
                return ServicesResult<DetailAppUser>.Failure(response.Message);
            }
            return ServicesResult<DetailAppUser>.Success(response.Data);
        }
        public async Task<ServicesResult<DetailAppUser>> UpdateUser(string token, UpdateAppUser user)
        {
            var resonseToken = _jwtServices.ParseToken(token);
            if (resonseToken.Status == false)
            {
                return ServicesResult<DetailAppUser>.Failure(resonseToken.Message);
            }
            var responseUpdate = await _userServices.UpdateUser(resonseToken.Data.UserId, user);
            if (responseUpdate.Status == false)
            {
                return ServicesResult<DetailAppUser>.Failure(responseUpdate.Message);
            }
            return ServicesResult<DetailAppUser>.Success(responseUpdate.Data);
        }
    }
}
