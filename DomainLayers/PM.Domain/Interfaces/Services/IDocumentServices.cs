using PM.Domain.Entities;

namespace PM.Domain.Interfaces.Services
{
    public interface IDocumentServices
    {
        public Task<ServicesResult<IEnumerable<Document>>> GetDocs();
        public Task<ServicesResult<IEnumerable<Document>>> GetDocsInProject(string projectId);
        public Task<ServicesResult<IEnumerable<Document>>> GetDocsInPlan(string planId);
        public Task<ServicesResult<IEnumerable<Document>>> GetDocsInMission(string missionId);
        public Task<ServicesResult<Document>> GetDoc(string docId);
        public Task<ServicesResult<bool>> AddDocToProject(Document newDoc);
        public Task<ServicesResult<bool>> AddDocToMission(Document newDoc);
        public Task<ServicesResult<bool>> UpdateDoc(Document updateDoc);
        public Task<ServicesResult<bool>> PatchDoc(string documentId, ProjectMember updateDocc);
        public Task<ServicesResult<bool>> DeleteDoc(string docId);
    }
}
