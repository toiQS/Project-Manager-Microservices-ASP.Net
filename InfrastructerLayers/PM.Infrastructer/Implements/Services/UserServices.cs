using Microsoft.Extensions.Logging;
using PM.Domain.Entities;
using PM.Domain;
using PM.Domain.Interfaces;

namespace PM.Infrastructer.Implements.Services
{
    public class UserServices
    {
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly ILogger<UserServices> _logger;
        public UserServices(IAuthUnitOfWork authUnitOfWork, ILogger<UserServices> logger)
        {
            _authUnitOfWork = authUnitOfWork;
            _logger = logger;
        }
        public Task<ServicesResult<User>> GetDetailUser(string userId)
        {
            if (userId == null)
            {
                _logger.LogError("");
            }
        }
        public Task<ServicesResult<bool>> UpdateUser(User user);
        public Task<ServicesResult<bool>> PacthUser(string userId, User user);
    }
}
