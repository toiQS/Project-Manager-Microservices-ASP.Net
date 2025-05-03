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

        // lấy kế hoạch trong dự án

        // chi tiết kế hoạch
        // thêm kế hoạch
        // chỉnh sửa kế hoạch
        // xóa kế hoạch
        // xóa nhiều kế hoạch cùng lúc
    }
}
