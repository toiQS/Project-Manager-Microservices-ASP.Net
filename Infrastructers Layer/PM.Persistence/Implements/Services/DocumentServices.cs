using Azure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using PM.Domain;
using PM.Domain.Interfaces;
using PM.Domain.Models.documents;
using PM.Domain.Models.plans;
using System.ComponentModel.DataAnnotations;
using System.Formats.Asn1;
using System.Runtime.Intrinsics.X86;

namespace PM.Persistence.Implements.Services
{
public    class DocumentServices
    {
        private readonly IUnitOfWork _unitOfWork;
        #region
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ServicesResult<IEnumerable<IndexDoc>>> GetDocs()
        {
            try
            {
                var docs = await _unitOfWork.DocumentRepository.GetAllAsync();
                if (docs.Status == false)
                    return ServicesResult<IEnumerable<IndexDoc>>.Failure(docs.Message);
                var response = docs.Data.Select(x => new IndexDoc
                {
                    Id = x.Id,
                    Name = x.Name,
                    Path = x.Path,
                });
                return ServicesResult<IEnumerable<IndexDoc>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexDoc>>.Failure($"{ex.Message}");

            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        #endregion

        #region
        public async Task<ServicesResult<IEnumerable<IndexDoc>>> GetDocsInProject(string projectId)
        {
            if (projectId == null) return ServicesResult<IEnumerable<IndexDoc>>.Failure("");
            try
            {
                var project = await _unitOfWork.ProjectRepository.ExistsAsync(projectId);
                if (project == false) return ServicesResult<IEnumerable<IndexDoc>>.Failure("");
                var plans = await _unitOfWork.PlanRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (plans.Status == false) return ServicesResult<IEnumerable<IndexDoc>>.Failure(plans.Message);
                var planIds = plans.Data.Select(x => x.Id);
                var missionIds = new List<string>();
                foreach (var item in planIds)
                {
                    var missions = await _unitOfWork.MissionRepository.GetManyByKeyAndValue("PlanId", item);
                    if (missions.Status == false) return ServicesResult<IEnumerable<IndexDoc>>.Failure(missions.Message);
                    var ids = missions.Data.Select(x => x.Id);
                    missionIds.AddRange(ids);
                }
                var response = new List<IndexDoc>();
                var docProjects = await _unitOfWork.DocumentRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (docProjects.Status == false) return ServicesResult<IEnumerable<IndexDoc>>.Failure(docProjects.Message);
                var responseDocProjects = docProjects.Data.Select(x => new IndexDoc()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Path = x.Path,
                });

                response.AddRange(responseDocProjects);

                foreach (var item in missionIds)
                {
                    var docMissions = await _unitOfWork.DocumentRepository.GetManyByKeyAndValue("MissionId", item);
                    if (docMissions.Status == false) return ServicesResult<IEnumerable<IndexDoc>>.Failure(docMissions.Message);
                    var responseDocMission = docMissions.Data.Select(x => new IndexDoc()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Path = x.Path,
                    });
                    response.AddRange(responseDocMission);
                }
                return ServicesResult<IEnumerable<IndexDoc>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexDoc>>.Failure("");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        #endregion

        #region
        /// <summary>
        /// 
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public async Task<ServicesResult<IEnumerable<IndexDoc>>> GetDocsInPlan(string planId)
        {
            if (planId == null) return ServicesResult<IEnumerable<IndexDoc>>.Failure("");
            try
            {
                var plan = await _unitOfWork.PlanRepository.GetOneByKeyAndValue("Id", planId);
                if (plan.Status == false) return ServicesResult<IEnumerable<IndexDoc>>.Failure(plan.Message);
                var missions = await _unitOfWork.MissionRepository.GetManyByKeyAndValue("PlanId", planId);
                if(missions.Status == false) return ServicesResult<IEnumerable<IndexDoc>>.Failure(missions.Message);
                var missionIds = missions.Data.Select(x => x.Id).ToList();
                var response = new List<IndexDoc>();    
                foreach (var item in missionIds)
                {
                    var docMissions = await _unitOfWork.DocumentRepository.GetManyByKeyAndValue("MissionId", item);
                    if(docMissions.Status == false) return ServicesResult<IEnumerable<IndexDoc>>.Failure(docMissions.Message);
                    var docMissionResponse = docMissions.Data.Select(x => new IndexDoc
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Path = x.Path,
                    });
                    response.AddRange(docMissionResponse);
                }
                return ServicesResult<IEnumerable<IndexDoc>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexDoc>>.Failure("");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        #endregion
        #region
        /// <summary>
        /// 
        /// </summary>
        /// <param name="missionId"></param>
        /// <returns></returns>
        public async Task<ServicesResult<IEnumerable<IndexDoc>>> GetDocsInMission(string missionId)
        {
            if (string.IsNullOrEmpty(missionId)) return ServicesResult<IEnumerable<IndexDoc>>.Failure("");
            try
            {
                var mission = await _unitOfWork.MissionRepository.GetOneByKeyAndValue("Id", missionId);
                if (mission.Status == false) return ServicesResult<IEnumerable<IndexDoc>>.Failure(mission.Message);
                var docMissions = await _unitOfWork.DocumentRepository.GetManyByKeyAndValue("MissionId", missionId);
                if (docMissions.Status == false) return ServicesResult<IEnumerable<IndexDoc>>.Failure(docMissions.Message);
                var responseDocMission = docMissions.Data.Select(x => new IndexDoc()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Path = x.Path,
                });
                return ServicesResult<IEnumerable<IndexDoc>>.Success(responseDocMission);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexDoc>>.Failure("");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        #endregion


        public Task<ServicesResult<DetailDoc>> GetDocs(string docId);
        public Task<ServicesResult<DetailDoc>> AddDocToProject(string memberId, string projectId, AddDoc addDoc);
        public Task<ServicesResult<DetailDoc>> AddDocToPlan(string memberId, string planId, AddDoc addDoc);
        public Task<ServicesResult<DetailDoc>> AddDocToMission(string memberId, string missionId, AddDoc addDoc);
        public Task<ServicesResult<DetailDoc>> UpdateDoc(string memberId, string docId, UpdateDoc updateDoc);
        public Task<ServicesResult<IEnumerable<IndexDoc>>> DeleteDoc(string memberId, string docId);
        
    }
}
