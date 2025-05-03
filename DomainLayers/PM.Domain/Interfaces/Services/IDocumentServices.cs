using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    /// <summary>
    /// Service interface for managing documents.
    /// </summary>
    public interface IDocumentServices
    {
        /// <summary>
        /// Retrieves all documents.
        /// </summary>
        /// <returns>A collection of documents.</returns>
        public Task<ServicesResult<IEnumerable<Document>>> GetDocs();

        /// <summary>
        /// Retrieves documents associated with a specific project.
        /// </summary>
        /// <param name="projectId">The unique identifier of the project.</param>
        /// <returns>A collection of documents for the specified project.</returns>
        public Task<ServicesResult<IEnumerable<Document>>> GetDocsInProject(string projectId);

        /// <summary>
        /// Retrieves documents associated with a specific plan.
        /// </summary>
        /// <param name="planId">The unique identifier of the plan.</param>
        /// <returns>A collection of documents for the specified plan.</returns>
        public Task<ServicesResult<IEnumerable<Document>>> GetDocsInPlan(string planId);

        /// <summary>
        /// Retrieves documents associated with a specific mission.
        /// </summary>
        /// <param name="missionId">The unique identifier of the mission.</param>
        /// <returns>A collection of documents for the specified mission.</returns>
        public Task<ServicesResult<IEnumerable<Document>>> GetDocsInMission(string missionId);

        /// <summary>
        /// Retrieves a specific document by its unique identifier.
        /// </summary>
        /// <param name="docId">The unique identifier of the document.</param>
        /// <returns>The document associated with the provided ID.</returns>
        public Task<ServicesResult<Document>> GetDoc(string docId);

        /// <summary>
        /// Adds a new document to a project.
        /// </summary>
        /// <param name="newDoc">The document to add.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> AddDocToProject(Document newDoc);

        /// <summary>
        /// Adds a new document to a mission.
        /// </summary>
        /// <param name="newDoc">The document to add.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> AddDocToMission(Document newDoc);

        /// <summary>
        /// Deletes a document.
        /// </summary>
        /// <param name="docId">The unique identifier of the document to delete.</param>
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> DeleteDoc(string docId);

        /// <summary>
        /// Applies partial updates to a document within a project.
        /// </summary>
        /// <param name="documentId">The unique identifier of the document.</param>
        /// <param name="updateDoc">The document data to patch.</param>
        /// <returns>True if the patch was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> PatchDocInProject(string documentId, Document updateDoc);

        /// <summary>
        /// Applies partial updates to a document within a mission.
        /// </summary>
        /// <param name="documentId">The unique identifier of the document.</param>
        /// <param name="updateDoc">The document data to patch.</param>
        /// <returns>True if the patch was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> PatchDocInMission(string documentId, Document updateDoc);

        /// <summary>
        /// Updates a document within a mission.
        /// </summary>
        /// <param name="updateDoc">The updated document data.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> UpdateDocInMission(Document updateDoc);

        /// <summary>
        /// Updates a document within a project.
        /// </summary>
        /// <param name="updateDoc">The updated document data.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        public Task<ServicesResult<bool>> UpdateDocInProject(Document updateDoc);
    }
}