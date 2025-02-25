using Microsoft.AspNetCore.Mvc;
using PM.Application.Models.docs;

namespace PM.Application.Interfaces
{
    public interface IDocumentLogic
    {
        public Task<IActionResult> GetDocs();
        public Task<IActionResult> GetDocsInProject(string projectId);
        public Task<IActionResult> GetDocsInPlan(string planId);
        public Task<IActionResult> GetDocsInMission(string missionId);
        public Task<IActionResult> GetDoc(string docId);
        public Task<IActionResult> AddDocToProject(AddDocToProjectModel model);
        public Task<IActionResult> AddDocToMission(AddDocToMissionModel model);
        public Task<IActionResult> UpdateDoc(UpdateDocModel model);
        public Task<IActionResult> DeleteDoc(DeleteDocModel model);
    }
}
