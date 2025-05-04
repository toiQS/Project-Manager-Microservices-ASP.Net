using PM.Shared.Dtos;
using PM.Shared.Dtos.tracking;

namespace PM.Tracking.Application.Interfaces
{
    public interface ITrackingHandle
    {
        public Task<ServiceResult<bool>> AddHandle(AddTrackingModel model);
    }
}
