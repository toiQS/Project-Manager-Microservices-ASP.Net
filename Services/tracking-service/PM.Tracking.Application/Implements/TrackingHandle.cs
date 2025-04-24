using PM.Shared.Dtos.auths;
using PM.Shared.Dtos.tracking;
using PM.Shared.Handle.Interfaces;
using PM.Tracking.Entities;
using PM.Tracking.Infrastructure.Data;
using System.Security.Principal;

namespace PM.Tracking.Application.Implements
{
    public class TrackingHandle
    {
        private readonly IUnitOfWork<TrackingDbContext> _unitOfWork;
        public async Task<ServiceResult<bool>> AddHandle(AddTrackingModel model)
        {
            if(model.ActionName == null)
            {
                return ServiceResult<bool>.Error("ActionName cannot be null");
            }
            var tracking = new ActivityLog()
            {
                Id =Guid.NewGuid().ToString(),
                UserId = model.UserId,
                ProjectId = model.ProjectId,
            };
            try
            {
                var addResult = await _unitOfWork.ExecuteTransactionAsync(async () => await _unitOfWork.Repository<ActivityLog>().AddAsync(tracking));
                if(addResult.Status != ResultStatus.Success)
                {
                    return ServiceResult<bool>.Error("Failed to add tracking");
                }
                var saveResult = await _unitOfWork.SaveChangesAsync();
                if (saveResult <= 0)
                {
                    return ServiceResult<bool>.Error("Failed to save changes");
                }
                return ServiceResult<bool>.Success(true, "Tracking added successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FromException(ex);
            }
        }
    }
}
