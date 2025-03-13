using EasyNetQ.Logging;
using PM.Domain.Entities;
using PM.Domain;
using PM.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace PM.Persistence.Implements.Services
{
    public class MissionAssignmentServices
    {
        private readonly ILogger<MissionAssignmentServices> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public MissionAssignmentServices(ILogger<MissionAssignmentServices> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<ServicesResult<IEnumerable<MissionAssignment>>> GetMissionAssignmentsAsync()
        {
            var response = await _unitOfWork.MissionAssignmentRepository.GetAllAsync(1,100);
            if (response.Status == false)
            {
                _logger.Error("");
                return ServicesResult<IEnumerable<MissionAssignment>>.Failure("");
            }
            return ServicesResult<IEnumerable<MissionAssignment>>.Success(response.Data!);
        }
        //public async Task<ServicesResult<IEnumerable<MissionAssignment>>> GetMissionAssignmentsInMissionAsync(string missionId)
        //{
        //    var response = await _unitOfWork.MissionAssignmentRepository.GetManyByKeyAndValue(Dictionary<string, string>{
        //        { "MissionId", missionId}
        //    }, true)
        //}
        //public Task<ServicesResult<MissionAssignment>> GetMissionAssignmentAsync(string missionAssistanceId);
        //public Task<ServicesResult<bool>> AddAsync(MissionAssignment missionAssignment);
        //public Task<ServicesResult<bool>> AddManyAsync(List<MissionAssignment> missionAssignment);
        //public Task<ServicesResult<bool>> RemoveAsync(MissionAssignment missionAssignment);
        //public Task<ServicesResult<bool>> UpdateAsync(MissionAssignment missionAssignment);
    }
}
