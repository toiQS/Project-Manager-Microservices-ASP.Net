using Microsoft.Identity.Client;
using Microsoft.VisualBasic;
using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain.Interfaces.Services
{
    public interface IStatusServices
    {
        public Task<ServicesResult<Status>> GetStatusAsync(int id);
        // current time and start time
        public Task<ServicesResult<string>> StatusForCreateAsync(DateTime currentDate, DateTime startDate); 
        public Task<ServicesResult<string>> StatusForUpdateAsync(DateTime startDate, DateTime endDate);
        public Task<ServicesResult<string>> StatusForFinallyAsync(DateTime currentTime, DateTime EndDate);
    }
}
