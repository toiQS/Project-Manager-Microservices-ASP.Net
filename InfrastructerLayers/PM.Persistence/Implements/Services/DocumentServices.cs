using Microsoft.Extensions.Logging;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;

namespace PM.Persistence.Implements.Services
{
    public class DocumentServices : IDocumentServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DocumentServices> _logger;
        public DocumentServices(IUnitOfWork unitOfWork, ILogger<DocumentServices> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        #region Get All Documents
        public async Task<ServicesResult<IEnumerable<Document>>> GetDocs()
        {
            _logger.LogInformation("[DocumentServices] Fetching all documents...");

            var response = await _unitOfWork.DocumentQueryRepository.GetAllAsync(1, 100);
            if (!response.Status)
            {
                _logger.LogError("[DocumentServices] GetDocs failed: {Message}", response.Message);
                return ServicesResult<IEnumerable<Document>>.Failure(response.Message);
            }

            _logger.LogInformation("[DocumentServices] Successfully fetched {Count} documents", response.Data?.Count());
            return ServicesResult<IEnumerable<Document>>.Success(response.Data!.ToList());
        }
        #endregion

        #region Get Documents in Project
        public async Task<ServicesResult<IEnumerable<Document>>> GetDocsInProject(string projectId)
        {
            _logger.LogInformation("[DocumentServices] Fetching documents for ProjectId={ProjectId}", projectId);

            var response = await _unitOfWork.DocumentQueryRepository.GetManyByKeyAndValue("ProjectId", projectId);
            if (!response.Status)
            {
                _logger.LogError("[DocumentServices] GetDocsInProject failed for ProjectId={ProjectId}: {Message}", projectId, response.Message);
                return ServicesResult<IEnumerable<Document>>.Failure(response.Message);
            }

            _logger.LogInformation("[DocumentServices] Successfully fetched {Count} documents for ProjectId={ProjectId}", response.Data?.Count(), projectId);
            return ServicesResult<IEnumerable<Document>>.Success(response.Data!.ToList());
        }
        #endregion

        #region Get Documents in Plan
        public async Task<ServicesResult<IEnumerable<Document>>> GetDocsInPlan(string planId)
        {
            _logger.LogInformation("[DocumentServices] Fetching documents for PlanId={PlanId}", planId);

            var missionsInPlanResponse = await _unitOfWork.MissionQueryRepository.GetManyByKeyAndValue("PlanId", planId);
            if (!missionsInPlanResponse.Status)
            {
                _logger.LogError("[DocumentServices] GetDocsInPlan failed for PlanId={PlanId}: {Message}", planId, missionsInPlanResponse.Message);
                return ServicesResult<IEnumerable<Document>>.Failure(missionsInPlanResponse.Message);
            }

            var response = new List<Document>();
            foreach (var item in missionsInPlanResponse.Data!)
            {
                var documents = await GetDocsInMission(item.Id);
                if (!documents.Status)
                {
                    _logger.LogError("[DocumentServices] Failed to get documents for MissionId={MissionId} in PlanId={PlanId}", item.Id, planId);
                    return ServicesResult<IEnumerable<Document>>.Failure(documents.Message);
                }
                response.AddRange(documents.Data!);
            }

            _logger.LogInformation("[DocumentServices] Successfully fetched {Count} documents for PlanId={PlanId}", response.Count, planId);
            return ServicesResult<IEnumerable<Document>>.Success(response);
        }
        #endregion

        #region Get Documents in Mission
        public async Task<ServicesResult<IEnumerable<Document>>> GetDocsInMission(string missionId)
        {
            _logger.LogInformation("[DocumentServices] Fetching documents for MissionId={MissionId}", missionId);

            var response = await _unitOfWork.DocumentQueryRepository.GetManyByKeyAndValue("MissionId", missionId);
            if (!response.Status)
            {
                _logger.LogError("[DocumentServices] GetDocsInMission failed for MissionId={MissionId}: {Message}", missionId, response.Message);
                return ServicesResult<IEnumerable<Document>>.Failure(response.Message);
            }

            _logger.LogInformation("[DocumentServices] Successfully fetched {Count} documents for MissionId={MissionId}", response.Data!.Count(), missionId);
            return ServicesResult<IEnumerable<Document>>.Success(response.Data!.ToList());
        }
        #endregion

        #region Get Document by Id
        public async Task<ServicesResult<Document>> GetDoc(string docId)
        {
            _logger.LogInformation("[DocumentServices] Fetching document with Id={DocId}", docId);

            var response = await _unitOfWork.DocumentQueryRepository.GetOneByKeyAndValue("Id", docId);
            if (!response.Status)
            {
                _logger.LogError("[DocumentServices] GetDoc failed for Id={DocId}: {Message}", docId, response.Message);
                return ServicesResult<Document>.Failure(response.Message);
            }

            _logger.LogInformation("[DocumentServices] Successfully fetched document with Id={DocId}", docId);
            return ServicesResult<Document>.Success(response.Data!);
        }
        #endregion


        #region Add Document to Project
        public async Task<ServicesResult<bool>> AddDocToProject(Document newDoc)
        {
            _logger.LogInformation("[DocumentServices] Adding new document to ProjectId={ProjectId}", newDoc.ProjectId);

            var docsProjectResponse = await GetDocsInProject(newDoc.ProjectId);
            if (!docsProjectResponse.Status)
            {
                _logger.LogError("[DocumentServices] AddDocToProject failed: {Message}", docsProjectResponse.Message);
                return ServicesResult<bool>.Failure(docsProjectResponse.Message);
            }

            var response = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.DocumentCommandRepository.AddAsync(docsProjectResponse.Data!.ToList(), newDoc)
            );

            if (!response.Status)
            {
                _logger.LogError("[DocumentServices] AddDocToProject failed: {Message}", response.Message);
                return ServicesResult<bool>.Failure(response.Message);
            }

            _logger.LogInformation("[DocumentServices] Successfully added document with Id={DocId} to ProjectId={ProjectId}", newDoc.Id, newDoc.ProjectId);
            return ServicesResult<bool>.Success(response.Data!);
        }
        #endregion


        #region Add Document to Mission
        public async Task<ServicesResult<bool>> AddDocToMission(Document newDoc)
        {
            _logger.LogInformation("[DocumentServices] Adding new document to MissionId={MissionId}", newDoc.MissionId);

            var docsMissionResponse = await GetDocsInMission(newDoc.MissionId);
            if (!docsMissionResponse.Status)
            {
                _logger.LogError("[DocumentServices] AddDocToMission failed: {Message}", docsMissionResponse.Message);
                return ServicesResult<bool>.Failure(docsMissionResponse.Message);
            }

            var response = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.DocumentCommandRepository.AddAsync(docsMissionResponse.Data!.ToList(), newDoc)
            );

            if (!response.Status)
            {
                _logger.LogError("[DocumentServices] AddDocToMission failed: {Message}", response.Message);
                return ServicesResult<bool>.Failure(response.Message);
            }

            _logger.LogInformation("[DocumentServices] Successfully added document with Id={DocId} to MissionId={MissionId}", newDoc.Id, newDoc.MissionId);
            return ServicesResult<bool>.Success(response.Data!);
        }
        #endregion


        #region Update Document
        public async Task<ServicesResult<bool>> UpdateDocInProject(Document updateDoc)
        {
            _logger.LogInformation("[DocumentServices] Updating document in ProjectId={ProjectId}", updateDoc.ProjectId);

            var docsProjectResponse = await GetDocsInProject(updateDoc.ProjectId);
            if (!docsProjectResponse.Status)
            {
                _logger.LogError("[DocumentServices] UpdateDocInProject failed: {Message}", docsProjectResponse.Message);
                return ServicesResult<bool>.Failure(docsProjectResponse.Message);
            }

            return await PrivateUpdateMethod(docsProjectResponse.Data!.ToList(), updateDoc);
        }

        public async Task<ServicesResult<bool>> UpdateDocInMission(Document updateDoc)
        {
            _logger.LogInformation("[DocumentServices] Updating document in MissionId={MissionId}", updateDoc.MissionId);

            var docsMissionResponse = await GetDocsInMission(updateDoc.MissionId);
            if (!docsMissionResponse.Status)
            {
                _logger.LogError("[DocumentServices] UpdateDocInMission failed: {Message}", docsMissionResponse.Message);
                return ServicesResult<bool>.Failure(docsMissionResponse.Message);
            }

            return await PrivateUpdateMethod(docsMissionResponse.Data!.ToList(), updateDoc);
        }

        private async Task<ServicesResult<bool>> PrivateUpdateMethod(List<Document> docs, Document document)
        {
            var response = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.DocumentCommandRepository.UpdateAsync(docs, document)
            );

            if (!response.Status)
            {
                _logger.LogError("[DocumentServices] PrivateUpdateMethod failed: {Message}", response.Message);
                return ServicesResult<bool>.Failure(response.Message);
            }

            _logger.LogInformation("[DocumentServices] Successfully updated document with Id={Id}", document.Id);
            return ServicesResult<bool>.Success(response.Data!);
        }
        #endregion

        #region Patch Document
        public async Task<ServicesResult<bool>> PatchDocInProject(string documentId, Document updateDoc)
        {
            _logger.LogInformation("[DocumentServices] Patching document in ProjectId={ProjectId}", updateDoc.ProjectId);

            var docsProjectResponse = await GetDocsInProject(updateDoc.ProjectId);
            if (!docsProjectResponse.Status)
            {
                _logger.LogError("[DocumentServices] PatchDocInProject failed: {Message}", docsProjectResponse.Message);
                return ServicesResult<bool>.Failure(docsProjectResponse.Message);
            }

            return await PrivatePatchMethod(docsProjectResponse.Data!.ToList(), documentId, updateDoc);
        }

        public async Task<ServicesResult<bool>> PatchDocInMission(string documentId, Document updateDoc)
        {
            _logger.LogInformation("[DocumentServices] Patching document in MissionId={MissionId}", updateDoc.MissionId);

            var docsMissionResponse = await GetDocsInMission(updateDoc.MissionId);
            if (!docsMissionResponse.Status)
            {
                _logger.LogError("[DocumentServices] PatchDocInMission failed: {Message}", docsMissionResponse.Message);
                return ServicesResult<bool>.Failure(docsMissionResponse.Message);
            }

            return await PrivatePatchMethod(docsMissionResponse.Data!.ToList(), documentId, updateDoc);
        }

        private async Task<ServicesResult<bool>> PrivatePatchMethod(List<Document> arr, string docId, Document document)
        {
            var valueKeys = new Dictionary<string, object>
            {
                {"Name", document.Name},
                {"Path", document.Path},
                {"Description", document.Description},
                {"MissionId", document.MissionId},
                {"ProjectId", document.ProjectId}
            };

            var response = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.DocumentCommandRepository.PatchAsync(arr, docId, valueKeys)
            );

            if (!response.Status)
            {
                _logger.LogError("[DocumentServices] PrivatePatchMethod failed: {Message}", response.Message);
                return ServicesResult<bool>.Failure(response.Message);
            }

            _logger.LogInformation("[DocumentServices] Successfully patched document with Id={Id}", docId);
            return ServicesResult<bool>.Success(response.Data!);
        }
        #endregion

        #region Delete Document
        public async Task<ServicesResult<bool>> DeleteDoc(string docId)
        {
            _logger.LogInformation("[DocumentServices] Deleting document with Id={DocId}", docId);

            var response = await _unitOfWork.ExecuteTransactionAsync(
                async () => await _unitOfWork.DocumentCommandRepository.DeleteAsync(docId)
            );

            if (!response.Status)
            {
                _logger.LogError("[DocumentServices] DeleteDoc failed: {Message}", response.Message);
                return ServicesResult<bool>.Failure(response.Message);
            }

            _logger.LogInformation("[DocumentServices] Successfully deleted document with Id={DocId}", docId);
            return ServicesResult<bool>.Success(response.Data!);
        }
        #endregion
    }
}
