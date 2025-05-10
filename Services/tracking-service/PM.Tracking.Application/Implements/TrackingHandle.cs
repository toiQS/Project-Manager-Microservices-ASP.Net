using PM.Tracking.Application.Interfaces;
using PM.Tracking.Entities;
using PM.Tracking.Infrastructure.Data;

namespace PM.Tracking.Application.Implements 
{
    public class TrackingHandle : ITrackingHandle
    {
        //private readonly IUnitOfWork<TrackingDbContext> _unitOfWork;
        //public TrackingHandle(IUnitOfWork<TrackingDbContext> unitOfWork)
        //{
        //    _unitOfWork = unitOfWork;
        //}

        //public async Task<ServiceResult<bool>> AddHandle(AddTrackingModel model)
        //{
        //    if(model.ActionName == null)
        //    {
        //        return ServiceResult<bool>.Error("ActionName cannot be null");
        //    }
        //    var tracking = new ActivityLog()
        //    {
        //        Id =Guid.NewGuid().ToString(),
        //        UserId = model.UserId,
        //        ProjectId = model.ProjectId,
        //        ActionName = model.ActionName,
        //        ActionAt = DateTime.Now,
        //    };
        //    try
        //    {
        //        var addResult = await _unitOfWork.ExecuteTransactionAsync( async () => await _unitOfWork.Repository<ActivityLog>().AddAsync(tracking));
        //        if(addResult.Status != ResultStatus.Success)
        //        {
        //            return ServiceResult<bool>.Error("Failed to add tracking");
        //        }
        //        var saveResult = await _unitOfWork.SaveChangesAsync();
        //        return ServiceResult<bool>.Success(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ServiceResult<bool>.FromException(ex);
        //    }
        //}
    }
}
