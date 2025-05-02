using Microsoft.SqlServer.Server;
using PM.Core.Application.Interfaces;
using PM.Core.Entities;
using PM.Core.Infrastructure.Data;
using PM.Shared.Dtos.auths;
using PM.Shared.Dtos.cores.plans;
using PM.Shared.Dtos.cores.projects;
using PM.Shared.Dtos.users;
using PM.Shared.Handle.Interfaces;

namespace PM.Core.Application.Implements
{
    public class PlanHandle
    {
        private readonly IUnitOfWork<CoreDbContext> _unitOfWork;
        private readonly IAPIService<UserDetail> _userAPI;
        private readonly IPositionHandle _positionHandle;
        private readonly Position _ownerPosition;
        public PlanHandle(IUnitOfWork<CoreDbContext> unitOfWork, IAPIService<UserDetail> userAPI, IPositionHandle positionHandle, Position ownerPosition)
        {
            _unitOfWork = unitOfWork;
            _userAPI = userAPI;
            _positionHandle = positionHandle;
            _ownerPosition = ownerPosition;
        }

        public async Task<ServiceResult<IEnumerable<IndexPlanModel>>> GetPlansProject(string projectId)
        {
            if (projectId == null) return ServiceResult<IEnumerable<IndexPlanModel>>.Error("");
            var result = new List<IndexPlanModel>();
            try
            {
                var plans = await _unitOfWork.Repository<Plan>().GetManyAsync("ProjectId", projectId);
                if (plans.Status != ResultStatus.Success) return ServiceResult<IEnumerable<IndexPlanModel>>.Error("");
                if (plans.Data == null) return ServiceResult<IEnumerable<IndexPlanModel>>.Success(result);
                result = plans.Data.Select(x => new IndexPlanModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Goal = x.Goal,
                }).ToList();
                return ServiceResult<IEnumerable<IndexPlanModel>>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<IndexPlanModel>>.FromException(ex);
            }
        }
        //public async Task<ServiceResult<DetailPlanModel>> GetDetailPlan(string planId)
        //{
        //    if (!string.IsNullOrEmpty(planId)) return ServiceResult<DetailPlanModel>.Error("");
        //    try
        //    {
        //        var plan = await _unitOfWork.Repository<Plan>().GetOneAsync("Id", planId);
        //        if (plan.Status != ResultStatus.Success || plan.Data == null) return ServiceResult<DetailPlanModel>.Error("");
        //        var result = new DetailPlanModel()
        //        {
        //            Id = plan.Data.Id,
        //            Name= plan.Data.Name,
        //            CreateBy = plan.Data.CreateBy,
        //            CreateDate = plan.Data.CreateDate,
        //            EndDate = plan.Data.EndDate,
        //            Goal
        //        };
        //    }
        //}
    }
}
