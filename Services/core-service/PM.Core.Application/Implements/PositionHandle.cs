using PM.Core.Entities;
using PM.Core.Infrastructure.Data;
using PM.Shared.Dtos;
using PM.Shared.Handle.Interfaces;

namespace PM.Core.Application.Implements
{
    public class PositionHandle
    {
        private readonly IUnitOfWork<CoreDbContext> _unitOfWork;
        public PositionHandle(IUnitOfWork<CoreDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<Position>> GetPositionByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return ServiceResult<Position>.Error("");
            }
            try
            {
                ServiceResult<Position> position = await _unitOfWork.Repository<Position>().GetOneAsync("Name", name);
                return position.Status != ResultStatus.Success || position.Data == null
                    ? ServiceResult<Position>.Error("")
                    : ServiceResult<Position>.Success(position.Data);
            }
            catch (Exception ex)
            {
                return ServiceResult<Position>.FromException(ex);
            }
        }
    }
}
