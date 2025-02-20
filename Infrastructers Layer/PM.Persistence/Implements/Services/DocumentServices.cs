using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Models.documents;
using System.ComponentModel.DataAnnotations;

namespace PM.Persistence.Implements.Services
{
    public class DocumentServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private string _ownerId;
        private string _managerId;
        private string _leaderId;
        private string _memberId;

        #region Retrieves all documents and maps them to IndexDoc objects.
        /// <summary>
        /// Retrieves all documents and maps them to IndexDoc objects.
        /// </summary>
        /// <returns>A service result containing a list of IndexDoc objects or an error message.</returns>
        public async Task<ServicesResult<IEnumerable<IndexDoc>>> GetDocs()
        {
            try
            {
                // Retrieve all documents from the repository
                var docsResult = await _unitOfWork.DocumentRepository.GetAllAsync();
                if (!docsResult.Status)
                    return ServicesResult<IEnumerable<IndexDoc>>.Failure(docsResult.Message);

                // Map each document to an IndexDoc
                var response = docsResult.Data.Select(doc => new IndexDoc
                {
                    Id = doc.Id,
                    Name = doc.Name,
                    Path = doc.Path,
                });

                return ServicesResult<IEnumerable<IndexDoc>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexDoc>>.Failure($"An error occurred: {ex.Message}");
            }
            finally
            {
                // Dispose the unit of work to free up resources
                _unitOfWork.Dispose();
            }
        }

        #endregion

        #region Retrieves all documents associated with a project, including those linked to plans and missions.
        /// <summary>
        /// Retrieves all documents associated with a project, including those linked to plans and missions.
        /// </summary>
        /// <param name="projectId">The ID of the project.</param>
        /// <returns>A service result containing a list of IndexDoc objects or an error message.</returns>
        public async Task<ServicesResult<IEnumerable<IndexDoc>>> GetDocsInProject(string projectId)
        {
            // Validate input parameter
            if (string.IsNullOrEmpty(projectId))
                return ServicesResult<IEnumerable<IndexDoc>>.Failure("Project ID cannot be null or empty.");

            try
            {
                // Check if the project exists
                bool projectExists = await _unitOfWork.ProjectRepository.ExistsAsync(projectId);
                if (!projectExists)
                    return ServicesResult<IEnumerable<IndexDoc>>.Failure("Project does not exist.");

                // Retrieve plans associated with the project
                var plansResult = await _unitOfWork.PlanRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (!plansResult.Status)
                    return ServicesResult<IEnumerable<IndexDoc>>.Failure(plansResult.Message);

                // Get plan IDs and initialize list for mission IDs
                var planIds = plansResult.Data.Select(x => x.Id);
                var missionIds = new List<string>();

                // Retrieve missions for each plan and collect their IDs
                foreach (var planId in planIds)
                {
                    var missionsResult = await _unitOfWork.MissionRepository.GetManyByKeyAndValue("PlanId", planId);
                    if (!missionsResult.Status)
                        return ServicesResult<IEnumerable<IndexDoc>>.Failure(missionsResult.Message);

                    missionIds.AddRange(missionsResult.Data.Select(x => x.Id));
                }

                // Initialize response list for documents
                var docsResponse = new List<IndexDoc>();

                // Retrieve documents directly associated with the project
                var projectDocsResult = await _unitOfWork.DocumentRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (!projectDocsResult.Status)
                    return ServicesResult<IEnumerable<IndexDoc>>.Failure(projectDocsResult.Message);

                docsResponse.AddRange(projectDocsResult.Data.Select(doc => new IndexDoc
                {
                    Id = doc.Id,
                    Name = doc.Name,
                    Path = doc.Path,
                }));

                // Retrieve documents associated with each mission
                foreach (var missionId in missionIds)
                {
                    var missionDocsResult = await _unitOfWork.DocumentRepository.GetManyByKeyAndValue("MissionId", missionId);
                    if (!missionDocsResult.Status)
                        return ServicesResult<IEnumerable<IndexDoc>>.Failure(missionDocsResult.Message);

                    docsResponse.AddRange(missionDocsResult.Data.Select(doc => new IndexDoc
                    {
                        Id = doc.Id,
                        Name = doc.Name,
                        Path = doc.Path,
                    }));
                }

                return ServicesResult<IEnumerable<IndexDoc>>.Success(docsResponse);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexDoc>>.Failure($"An error occurred: {ex.Message}");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        #endregion

        #region Retrieves all documents associated with a specific plan by gathering documents from each mission in the plan.
        /// <summary>
        /// Retrieves all documents associated with a specific plan by gathering documents from each mission in the plan.
        /// </summary>
        /// <param name="planId">The ID of the plan.</param>
        /// <returns>A service result containing a list of IndexDoc objects or an error message.</returns>
        public async Task<ServicesResult<IEnumerable<IndexDoc>>> GetDocsInPlan(string planId)
        {
            // Validate input parameter
            if (string.IsNullOrEmpty(planId))
                return ServicesResult<IEnumerable<IndexDoc>>.Failure("Plan ID cannot be null or empty.");

            try
            {
                // Retrieve the plan details
                var planResult = await _unitOfWork.PlanRepository.GetOneByKeyAndValue("Id", planId);
                if (!planResult.Status)
                    return ServicesResult<IEnumerable<IndexDoc>>.Failure($"Failed to retrieve plan: {planResult.Message}");

                // Retrieve missions associated with the plan
                var missionsResult = await _unitOfWork.MissionRepository.GetManyByKeyAndValue("PlanId", planId);
                if (!missionsResult.Status)
                    return ServicesResult<IEnumerable<IndexDoc>>.Failure($"Failed to retrieve missions: {missionsResult.Message}");

                // Collect mission IDs
                var missionIds = missionsResult.Data.Select(x => x.Id).ToList();
                var docsResponse = new List<IndexDoc>();

                // Iterate over each mission ID and retrieve its documents
                foreach (var missionId in missionIds)
                {
                    var docsResult = await _unitOfWork.DocumentRepository.GetManyByKeyAndValue("MissionId", missionId);
                    if (!docsResult.Status)
                        return ServicesResult<IEnumerable<IndexDoc>>.Failure($"Failed to retrieve documents for mission {missionId}: {docsResult.Message}");

                    // Map each document to an IndexDoc object and add to the response list
                    var missionDocs = docsResult.Data.Select(x => new IndexDoc
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Path = x.Path,
                    });
                    docsResponse.AddRange(missionDocs);
                }

                return ServicesResult<IEnumerable<IndexDoc>>.Success(docsResponse);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexDoc>>.Failure($"An error occurred: {ex.Message}");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        #endregion

        #region Retrieves all documents associated with a specific mission.
        /// <summary>
        /// Retrieves all documents associated with a specific mission.
        /// </summary>
        /// <param name="missionId">The ID of the mission.</param>
        /// <returns>A service result containing a list of IndexDoc objects or an error message.</returns>
        public async Task<ServicesResult<IEnumerable<IndexDoc>>> GetDocsInMission(string missionId)
        {
            // Validate input
            if (string.IsNullOrEmpty(missionId))
                return ServicesResult<IEnumerable<IndexDoc>>.Failure("Mission ID cannot be null or empty.");

            try
            {
                // Retrieve mission details (ensuring the mission exists)
                var missionResult = await _unitOfWork.MissionRepository.GetOneByKeyAndValue("Id", missionId);
                if (!missionResult.Status)
                    return ServicesResult<IEnumerable<IndexDoc>>.Failure($"Failed to retrieve mission: {missionResult.Message}");

                // Retrieve documents associated with the mission
                var docsResult = await _unitOfWork.DocumentRepository.GetManyByKeyAndValue("MissionId", missionId);
                if (!docsResult.Status)
                    return ServicesResult<IEnumerable<IndexDoc>>.Failure($"Failed to retrieve documents: {docsResult.Message}");

                // Map documents to IndexDoc objects
                var responseDocs = docsResult.Data.Select(doc => new IndexDoc
                {
                    Id = doc.Id,
                    Name = doc.Name,
                    Path = doc.Path,
                });

                return ServicesResult<IEnumerable<IndexDoc>>.Success(responseDocs);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexDoc>>.Failure($"An error occurred: {ex.Message}");
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
        /// <param name="docId"></param>
        /// <returns></returns>
        public async Task<ServicesResult<DetailDoc>> GetDoc(string docId)
        {
            if (string.IsNullOrEmpty(docId))
                return ServicesResult<DetailDoc>.Failure("");
            try
            {
                var doc = await _unitOfWork.DocumentRepository.GetOneByKeyAndValue("Id", docId);
                if (doc.Status == false) return ServicesResult<DetailDoc>.Failure(doc.Message);
                var response = new DetailDoc()
                {
                    Id = doc.Data.Id,
                    Name = doc.Data.Name,
                    Description = doc.Data.Descriotion,
                    Path = doc.Data.Path,
                };
                return ServicesResult<DetailDoc>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailDoc>.Failure("");
            }
            finally { _unitOfWork.Dispose(); }
        }
        #endregion
        #region
        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="projectId"></param>
        /// <param name="addDoc"></param>
        /// <returns></returns>
        public async Task<ServicesResult<DetailDoc>> AddDocToProject(string memberId, string projectId, AddDoc addDoc)
        {
            if (string.IsNullOrEmpty(memberId) || string.IsNullOrEmpty(projectId) || addDoc is null)
            {
                return ServicesResult<DetailDoc>.Failure("");
            }
            try
            {
                var project = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", projectId);
                if (project.Status == false) return ServicesResult<DetailDoc>.Failure(project.Message);
                var member = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberId);
                if (member.Status == false)
                    return ServicesResult<DetailDoc>.Failure(member.Message);
                if (member.Data.ProjectId != projectId || member.Data.RoleId != memberId)
                    return ServicesResult<DetailDoc>.Failure("");
                var doc = new Document()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = addDoc.Name,
                    Descriotion = addDoc.Description,
                };
                var addDocumentResponse = await _unitOfWork.DocumentRepository.AddAsync(doc);
                if (addDocumentResponse.Status == false) return ServicesResult<DetailDoc>.Failure(addDocumentResponse.Message);

                var infoMember = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", member.Data.UserId);
                if (infoMember.Status == false) return ServicesResult<DetailDoc>.Failure(infoMember.Message);
                var log = new ActivityLog()
                {
                    Id = Guid.NewGuid().ToString(),
                    Action = $"a new doccument was added by {infoMember.Data.UserName} in project {project.Data.Name}",
                    UserId = member.Data.UserId,
                    ProjectId = projectId,
                };
                var addLogResponse = await _unitOfWork.ActivityLogRepository.AddAsync(log);
                if (addLogResponse.Status == false) return ServicesResult<DetailDoc>.Failure(addLogResponse.Message);
                return await GetDoc(doc.Id);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailDoc>.Failure(ex.Message);
            }
        }
        #endregion
        public Task<ServicesResult<DetailDoc>> AddDocToMission(string memberId, string missionId, AddDoc addDoc);
        public Task<ServicesResult<DetailDoc>> UpdateDoc(string memberId, string docId, UpdateDoc updateDoc);
        public Task<ServicesResult<IEnumerable<IndexDoc>>> DeleteDoc(string memberId, string docId);

        #region Private method helper
        /// <summary>
        /// Gets the role ID by role name.
        /// </summary>
        /// <param name="roleName">The name of the role to fetch.</param>
        /// <returns>Service result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> GetRoleByName(string roleName)
        {
            try
            {
                var role = await _unitOfWork.RoleInProjectRepository.GetOneByKeyAndValue("Name", roleName);
                if (!role.Status)
                    return ServicesResult<bool>.Failure(role.Message);

                // Assign the role ID to the appropriate variable
                switch (roleName)
                {
                    case "Owner":
                        _ownerId = role.Data.Id;
                        break;
                    case "Leader":
                        _leaderId = role.Data.Id;
                        break;
                    case "Manager":
                        _managerId = role.Data.Id;
                        break;
                    case "Member":
                        _memberId = role.Data.Id;
                        break;
                    default:
                        return ServicesResult<bool>.Failure("Invalid role name");
                }

                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Failure(ex.Message);
            }
            finally
            {
                // Remove this if you don’t want to dispose the unit of work here.
                _unitOfWork.Dispose();
            }
        }

        /// <summary>
        /// Gets the role ID for the "Owner" role.
        /// </summary>
        /// <returns>Service result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> GetOwnRole()
        {
            return await GetRoleByName("Owner");
        }

        /// <summary>
        /// Gets the role ID for the "Leader" role.
        /// </summary>
        /// <returns>Service result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> GetLeaderRole()
        {
            return await GetRoleByName("Leader");
        }

        /// <summary>
        /// Gets the role ID for the "Manager" role.
        /// </summary>
        /// <returns>Service result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> GetManagerRole()
        {
            return await GetRoleByName("Manager");
        }

        /// <summary>
        /// Gets the role ID for the "Member" role.
        /// </summary>
        /// <returns>Service result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> GetMemberRole()
        {
            return await GetRoleByName("Member");
        }


        #endregion

    }
}
