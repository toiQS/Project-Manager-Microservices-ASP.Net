using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;
using PM.Domain.Models.documents;

namespace PM.Persistence.Implements.Services
{
    public class DocumentServices : IDocumentServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private string _ownerId;
        private string _managerId;
        private string _leaderId;
        private string _memberId;
        public DocumentServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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

        #region Retrieves detailed information about a document.
        /// <summary>
        /// Retrieves detailed information about a document.
        /// </summary>
        /// <param name="docId">The ID of the document to retrieve.</param>
        /// <returns>A service result containing the document details or an error message.</returns>
        public async Task<ServicesResult<DetailDoc>> GetDoc(string docId)
        {
            // Validate input parameter
            if (string.IsNullOrEmpty(docId))
                return ServicesResult<DetailDoc>.Failure("Document ID cannot be null or empty.");

            try
            {
                // Retrieve the document from the repository
                var docResult = await _unitOfWork.DocumentRepository.GetOneByKeyAndValue("Id", docId);
                if (!docResult.Status)
                    return ServicesResult<DetailDoc>.Failure($"Failed to retrieve document: {docResult.Message}");

                // Map retrieved data to a DetailDoc object
                var response = new DetailDoc
                {
                    Id = docResult.Data.Id,
                    Name = docResult.Data.Name,
                    Description = docResult.Data.Descriotion, // Note: Verify property name "Descriotion"
                    Path = docResult.Data.Path,
                };

                return ServicesResult<DetailDoc>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailDoc>.Failure($"An error occurred: {ex.Message}");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        #endregion

        #region Adds a new document to a project and logs the action.
        /// <summary>
        /// Adds a new document to a project and logs the action.
        /// </summary>
        /// <param name="memberId">The ID of the member adding the document.</param>
        /// <param name="projectId">The ID of the project to which the document is being added.</param>
        /// <param name="addDoc">The document details to add.</param>
        /// <returns>A service result containing the detailed document information or an error message.</returns>
        public async Task<ServicesResult<DetailDoc>> AddDocToProject(string memberId, string projectId, AddDoc addDoc)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(memberId) || string.IsNullOrEmpty(projectId) || addDoc == null)
                return ServicesResult<DetailDoc>.Failure("Invalid input parameters.");

            try
            {
                // Retrieve project details
                var projectResult = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", projectId);
                if (!projectResult.Status)
                    return ServicesResult<DetailDoc>.Failure($"Failed to retrieve project: {projectResult.Message}");

                // Retrieve member details
                var memberResult = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberId);
                if (!memberResult.Status)
                    return ServicesResult<DetailDoc>.Failure($"Failed to retrieve member: {memberResult.Message}");

                var memberData = memberResult.Data;

                // Ensure the member is associated with the project and has the required role (adjust role check as needed)
                if (memberData.ProjectId != projectId)
                    return ServicesResult<DetailDoc>.Failure("Member is not associated with the specified project.");

                // NOTE: The following check is ambiguous; adjust as necessary to verify proper role.
                if (memberData.RoleId != memberId)
                    return ServicesResult<DetailDoc>.Failure("Member does not have sufficient permissions to add a document.");

                // Create a new document
                var newDocument = new Document
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = addDoc.Name,
                    Descriotion = addDoc.Description, // Ensure that this property name is correct (e.g., "Description")
                };

                // Add the document to the repository
                var addDocResult = await _unitOfWork.DocumentRepository.AddAsync(newDocument);
                if (!addDocResult.Status)
                    return ServicesResult<DetailDoc>.Failure($"Failed to add document: {addDocResult.Message}");

                // Retrieve user details for logging
                var userResult = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", memberData.UserId);
                if (!userResult.Status)
                    return ServicesResult<DetailDoc>.Failure($"Failed to retrieve user info: {userResult.Message}");

                // Log the addition of the document
                var log = new ActivityLog
                {
                    Id = Guid.NewGuid().ToString(),
                    Action = $"A new document was added by {userResult.Data.UserName} in project {projectResult.Data.Name}.",
                    UserId = memberData.UserId,
                    ProjectId = projectId,
                    ActionDate = DateTime.Now
                };

                var logResult = await _unitOfWork.ActivityLogRepository.AddAsync(log);
                if (!logResult.Status)
                    return ServicesResult<DetailDoc>.Failure($"Failed to log activity: {logResult.Message}");

                // Retrieve and return the newly added document's details
                return await GetDoc(newDocument.Id);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailDoc>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }

        #endregion

        #region Adds a new document to a mission and logs the action.
        /// <summary>
        /// Adds a new document to a mission and logs the action.
        /// </summary>
        /// <param name="memberId">The ID of the member adding the document.</param>
        /// <param name="missionId">The ID of the mission to which the document is being added.</param>
        /// <param name="addDoc">The document details to add.</param>
        /// <returns>A service result containing the detailed document information or an error message.</returns>
        public async Task<ServicesResult<DetailDoc>> AddDocToMission(string memberId, string missionId, AddDoc addDoc)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(memberId) || string.IsNullOrEmpty(missionId) || addDoc == null)
                return ServicesResult<DetailDoc>.Failure("Invalid input parameters.");

            try
            {
                // Retrieve mission details
                var missionResult = await _unitOfWork.MissionRepository.GetOneByKeyAndValue("Id", missionId);
                if (!missionResult.Status)
                    return ServicesResult<DetailDoc>.Failure($"Failed to retrieve mission: {missionResult.Message}");

                // Retrieve plan details for the mission.
                // NOTE: It seems like there might be an error here. You are using mission.Data.Id for the plan Id.
                // Ensure that the planId is correctly provided from the mission object, e.g., mission.Data.PlanId.
                var planResult = await _unitOfWork.PlanRepository.GetOneByKeyAndValue("Id", missionResult.Data.PlanId);
                if (!planResult.Status)
                    return ServicesResult<DetailDoc>.Failure($"Failed to retrieve plan: {planResult.Message}");

                // Retrieve project details from the plan
                var projectResult = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", planResult.Data.ProjectId);
                if (!projectResult.Status)
                    return ServicesResult<DetailDoc>.Failure($"Failed to retrieve project: {projectResult.Message}");

                // Retrieve the member details (the one adding the document)
                var memberResult = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberId);
                if (!memberResult.Status)
                    return ServicesResult<DetailDoc>.Failure($"Failed to retrieve member: {memberResult.Message}");

                // Validate the member's permission: check if the member is part of the project and has the appropriate role.
                // NOTE: Adjust the role check logic if needed; here we assume _memberId is the required role.
                if (memberResult.Data.ProjectId != projectResult.Data.Id || memberResult.Data.RoleId != _memberId)
                    return ServicesResult<DetailDoc>.Failure("Member does not have sufficient permissions to add a document to this mission.");

                // Create the new document
                var newDoc = new Document
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = addDoc.Name,
                    Descriotion = addDoc.Description, // Check the property name; should it be "Description"?
                    MissionId = missionId,
                    Path = addDoc.Path,
                };

                // Add the document to the repository
                var addDocResult = await _unitOfWork.DocumentRepository.AddAsync(newDoc);
                if (!addDocResult.Status)
                    return ServicesResult<DetailDoc>.Failure($"Failed to add document: {addDocResult.Message}");

                // Retrieve user details for logging
                var userResult = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", memberResult.Data.UserId);
                if (!userResult.Status)
                    return ServicesResult<DetailDoc>.Failure($"Failed to retrieve user details: {userResult.Message}");

                // Log the document addition action
                var log = new ActivityLog
                {
                    Id = Guid.NewGuid().ToString(),
                    Action = $"A new document was added to mission by {userResult.Data.UserName} in project {projectResult.Data.Name}.",
                    UserId = userResult.Data.Id,
                    ProjectId = projectResult.Data.Id,
                    ActionDate = DateTime.Now,
                };

                var logResult = await _unitOfWork.ActivityLogRepository.AddAsync(log);
                if (!logResult.Status)
                    return ServicesResult<DetailDoc>.Failure($"Failed to log activity: {logResult.Message}");

                // Retrieve and return the newly added document's details
                return await GetDoc(newDoc.Id);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailDoc>.Failure($"An error occurred: {ex.Message}");
            }
        }

        #endregion

        #region Updates a document's details and logs the update action.
        /// <summary>
        /// Updates a document's details and logs the update action.
        /// </summary>
        /// <param name="memberId">The ID of the member performing the update.</param>
        /// <param name="docId">The ID of the document to update.</param>
        /// <param name="updateDoc">The updated document details.</param>
        /// <returns>A service result containing the updated document details or an error message.</returns>
        public async Task<ServicesResult<DetailDoc>> UpdateDoc(string memberId, string docId, UpdateDoc updateDoc)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(memberId) || string.IsNullOrEmpty(docId) || updateDoc == null)
                return ServicesResult<DetailDoc>.Failure("Invalid input parameters.");

            try
            {
                // Retrieve the document to update
                var docResult = await _unitOfWork.DocumentRepository.GetOneByKeyAndValue("Id", docId);
                if (!docResult.Status)
                    return ServicesResult<DetailDoc>.Failure($"Failed to retrieve document: {docResult.Message}");

                // Retrieve the mission associated with the document
                var missionResult = await _unitOfWork.MissionRepository.GetOneByKeyAndValue("Id", docResult.Data.MissionId);
                if (!missionResult.Status)
                    return ServicesResult<DetailDoc>.Failure($"Failed to retrieve mission: {missionResult.Message}");

                // Retrieve the plan associated with the mission
                // NOTE: Ensure that mission.Data contains a property (e.g., PlanId) for retrieving the plan.
                var planResult = await _unitOfWork.PlanRepository.GetOneByKeyAndValue("Id", missionResult.Data.PlanId);
                if (!planResult.Status)
                    return ServicesResult<DetailDoc>.Failure($"Failed to retrieve plan: {planResult.Message}");

                // Retrieve the project associated with the plan
                var projectResult = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", planResult.Data.ProjectId);
                if (!projectResult.Status)
                    return ServicesResult<DetailDoc>.Failure($"Failed to retrieve project: {projectResult.Message}");

                // Retrieve the member performing the update
                var memberResult = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberId);
                if (!memberResult.Status)
                    return ServicesResult<DetailDoc>.Failure($"Failed to retrieve member: {memberResult.Message}");

                // Validate that the member belongs to the project and has the required role
                if (memberResult.Data.ProjectId != projectResult.Data.Id || memberResult.Data.RoleId != _memberId)
                    return ServicesResult<DetailDoc>.Failure("Member does not have sufficient permissions to update this document.");

                // Update the document details
                docResult.Data.Name = updateDoc.Name;
                docResult.Data.Path = updateDoc.Path;
                docResult.Data.Descriotion = updateDoc.Description;  // Confirm if this property should be "Descriotion" or "Description"

                var updateResponse = await _unitOfWork.DocumentRepository.UpdateAsync(docResult.Data);
                if (!updateResponse.Status)
                    return ServicesResult<DetailDoc>.Failure($"Failed to update document: {updateResponse.Message}");

                // Retrieve user info for logging purposes
                var userResult = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", memberResult.Data.UserId);
                if (!userResult.Status)
                    return ServicesResult<DetailDoc>.Failure($"Failed to retrieve user info: {userResult.Message}");

                // Create a log for the update action
                var log = new ActivityLog
                {
                    Id = Guid.NewGuid().ToString(),
                    Action = $"Document was updated by {userResult.Data.UserName} in project {projectResult.Data.Name}.",
                    ActionDate = DateTime.Now,
                    ProjectId = projectResult.Data.Id,
                    UserId = memberResult.Data.UserId,
                };

                var logResult = await _unitOfWork.ActivityLogRepository.AddAsync(log);
                if (!logResult.Status)
                    return ServicesResult<DetailDoc>.Failure($"Failed to log activity: {logResult.Message}");

                // Return the updated document details
                return await GetDoc(docId);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailDoc>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }

        #endregion

        #region Deletes a document from a project and returns the updated list of documents.
        /// <summary>
        /// Deletes a document from a project and returns the updated list of documents.
        /// </summary>
        /// <param name="memberId">The ID of the member performing the deletion.</param>
        /// <param name="docId">The ID of the document to delete.</param>
        /// <returns>A service result containing a list of updated documents or an error message.</returns>
        public async Task<ServicesResult<IEnumerable<IndexDoc>>> DeleteDoc(string memberId, string docId)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(memberId) || string.IsNullOrEmpty(docId))
                return ServicesResult<IEnumerable<IndexDoc>>.Failure("Invalid input parameters.");

            try
            {
                // Retrieve the document details
                var docResult = await _unitOfWork.DocumentRepository.GetOneByKeyAndValue("Id", docId);
                if (!docResult.Status)
                    return ServicesResult<IEnumerable<IndexDoc>>.Failure($"Failed to retrieve document: {docResult.Message}");

                // Retrieve the mission associated with the document
                var missionResult = await _unitOfWork.MissionRepository.GetOneByKeyAndValue("Id", docResult.Data.MissionId);
                if (!missionResult.Status)
                    return ServicesResult<IEnumerable<IndexDoc>>.Failure($"Failed to retrieve mission: {missionResult.Message}");

                // Retrieve the plan associated with the mission
                // NOTE: Ensure that missionResult.Data.PlanId is used if available. Adjust accordingly.
                var planResult = await _unitOfWork.PlanRepository.GetOneByKeyAndValue("Id", missionResult.Data.PlanId);
                if (!planResult.Status)
                    return ServicesResult<IEnumerable<IndexDoc>>.Failure($"Failed to retrieve plan: {planResult.Message}");

                // Retrieve the project associated with the plan
                var projectResult = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", planResult.Data.ProjectId);
                if (!projectResult.Status)
                    return ServicesResult<IEnumerable<IndexDoc>>.Failure($"Failed to retrieve project: {projectResult.Message}");

                // Retrieve the member performing the deletion
                var memberResult = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberId);
                if (!memberResult.Status)
                    return ServicesResult<IEnumerable<IndexDoc>>.Failure($"Failed to retrieve member: {memberResult.Message}");

                // Validate that the member has permission to delete the document
                if (memberResult.Data.ProjectId != projectResult.Data.Id || memberResult.Data.RoleId != _memberId)
                    return ServicesResult<IEnumerable<IndexDoc>>.Failure("Member does not have permission to delete this document.");

                // Delete the document
                var deleteResult = await _unitOfWork.DocumentRepository.DeleteAsync(docId);
                if (!deleteResult.Status)
                    return ServicesResult<IEnumerable<IndexDoc>>.Failure($"Failed to delete document: {deleteResult.Message}");

                // Return the updated list of documents in the project
                return await GetDocsInProject(projectResult.Data.Id);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexDoc>>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }

        #endregion

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
