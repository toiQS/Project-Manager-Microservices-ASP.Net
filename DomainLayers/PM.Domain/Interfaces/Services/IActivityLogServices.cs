using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    public interface IActivityLogServices
    {
        
        public Task<ServicesResult<bool>> AddAsync(ActivityLog log);
        
    }
}
