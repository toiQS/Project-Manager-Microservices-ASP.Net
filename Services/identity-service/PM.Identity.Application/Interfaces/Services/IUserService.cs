using PM.Identity.Domain.Entities;
using PM.Shared.Dtos;

namespace PM.Identity.Application.Interfaces.Services
{
    public interface IUserService
    {
        //public Task<ServiceResult<User>> 
        public Task<ServiceResult<User>> GetUserByEmail(string email);
        public Task<ServiceResult<User>> GetUserById(string id);
    }
}
