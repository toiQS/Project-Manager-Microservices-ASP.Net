using PM.Core.Entities;
using PM.Core.Infrastructure.Data;
using PM.Shared.Dtos.auths;
using PM.Shared.Handle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                var position = await _unitOfWork.Repository<Position>().GetOneAsync("Name", name);
                if (position.Status != ResultStatus.Success || position.Data == null)
                {
                    return ServiceResult<Position>.Error("");
                }

                return ServiceResult<Position>.Success(position.Data);
            }
            catch (Exception ex)
            {
                return ServiceResult<Position>.FromException(ex);
            }
        }
    }
}
