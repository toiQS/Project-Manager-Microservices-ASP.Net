using PM.Core.Application.Interfaces;
using PM.Core.Entities;
using PM.Core.Infrastructure.Data;
using PM.Shared.Dtos;
using PM.Shared.Dtos.cores.members;
using PM.Shared.Dtos.users;
using PM.Shared.Handle.Interfaces;

namespace PM.Core.Application.Implements
{
    public class ProjectMemberHandle
    {
        private readonly IUnitOfWork<CoreDbContext> _unitOfWork;
        private readonly IAPIService<UserDetail> _userAPI;
        private readonly IPositionHandle _positionHandle;
        private readonly Position _ownerPosition;
        public ProjectMemberHandle(IUnitOfWork<CoreDbContext> unitOfWork, IAPIService<UserDetail> userAPI, IPositionHandle positionHandle)
        {
            _unitOfWork = unitOfWork;
            _userAPI = userAPI;
            _positionHandle = positionHandle;
            _ownerPosition = _positionHandle.GetPositionByName("Product Owner").GetAwaiter().GetResult().Data;
        }
        

    }
}
