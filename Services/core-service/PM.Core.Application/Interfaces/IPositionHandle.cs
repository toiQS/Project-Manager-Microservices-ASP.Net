using PM.Core.Entities;
using PM.Shared.Dtos.auths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Core.Application.Interfaces
{
    public interface IPositionHandle
    {
        Task<ServiceResult<Position>> GetPositionByName(string name);
    }
}
