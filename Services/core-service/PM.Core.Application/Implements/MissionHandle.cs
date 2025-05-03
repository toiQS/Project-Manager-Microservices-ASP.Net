using PM.Core.Infrastructure.Data;
using PM.Shared.Dtos.users;
using PM.Shared.Handle.Interfaces;

namespace PM.Core.Application.Implements
{
    public class MissionHandle
    {
        private readonly IUnitOfWork<CoreDbContext> _unitOfWork;
        private readonly IAPIService<UserDetail> _userAPI;
        public MissionHandle(IUnitOfWork<CoreDbContext> unitOfWork, IAPIService<UserDetail> userAPI)
        {
            _unitOfWork = unitOfWork;
            _userAPI = userAPI;
        }
    }
}
