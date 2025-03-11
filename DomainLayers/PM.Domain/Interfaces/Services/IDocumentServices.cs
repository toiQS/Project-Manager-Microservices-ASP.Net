using PM.Domain.Models.documents;

namespace PM.Domain.Interfaces.Services
{
    public interface IDocumentServices
    {
        public Task<ServicesResult<IEnumerable<IndexDoc>>> GetDocs();
        public Task<ServicesResult<IEnumerable<IndexDoc>>> GetDocsInProject(string projectId);
        public Task<ServicesResult<IEnumerable<IndexDoc>>> GetDocsInPlan(string planId);
        public Task<ServicesResult<IEnumerable<IndexDoc>>> GetDocsInMission(string missionId);
        public Task<ServicesResult<DetailDoc>> GetDoc(string docId);
        public Task<ServicesResult<DetailDoc>> AddDocToProject(string memberId, string projectId, AddDoc addDoc);
        public Task<ServicesResult<DetailDoc>> AddDocToMission(string memberId, string missionId, AddDoc addDoc);
        public Task<ServicesResult<DetailDoc>> UpdateDoc(string memberId, string docId, UpdateDoc updateDoc);
        public Task<ServicesResult<IEnumerable<IndexDoc>>> DeleteDoc(string memberId, string docId);
        public Task<ServicesResult<bool>> DeleteDocFunc(string memberId, string docId);
    }
}
