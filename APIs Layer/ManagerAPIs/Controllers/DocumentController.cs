using Microsoft.AspNetCore.Mvc;
using PM.Application.Interfaces;
using PM.Application.Models.docs;
using System.Threading.Tasks;

namespace ManagerAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentLogic _documentLogic;

        public DocumentController(IDocumentLogic documentLogic)
        {
            _documentLogic = documentLogic;
        }

        #region GetDocs
        /// <summary>
        /// Retrieves all documents.
        /// </summary>
        [HttpGet("get-docs")]
        public async Task<IActionResult> GetDocs()
        {
            return await _documentLogic.GetDocs();
        }
        #endregion

        #region GetDocsInProject
        /// <summary>
        /// Retrieves all documents in a specific project.
        /// </summary>
        [HttpGet("get-docs-in-project")]
        public async Task<IActionResult> GetDocsInProject([FromQuery] string projectId)
        {
            return await _documentLogic.GetDocsInProject(projectId);
        }
        #endregion

        #region GetDocsInPlan
        /// <summary>
        /// Retrieves all documents in a specific plan.
        /// </summary>
        [HttpGet("get-docs-in-plan")]
        public async Task<IActionResult> GetDocsInPlan([FromQuery] string planId)
        {
            return await _documentLogic.GetDocsInPlan(planId);
        }
        #endregion

        #region GetDocsInMission
        /// <summary>
        /// Retrieves all documents in a specific mission.
        /// </summary>
        [HttpGet("get-docs-in-mission")]
        public async Task<IActionResult> GetDocsInMission([FromQuery] string missionId)
        {
            return await _documentLogic.GetDocsInMission(missionId);
        }
        #endregion

        #region GetDoc
        /// <summary>
        /// Retrieves a specific document by ID.
        /// </summary>
        [HttpGet("get-doc")]
        public async Task<IActionResult> GetDoc([FromQuery] string docId)
        {
            return await _documentLogic.GetDoc(docId);
        }
        #endregion

        #region AddDocToProject
        /// <summary>
        /// Adds a document to a project.
        /// </summary>
        [HttpPost("add-doc-to-project")]
        public async Task<IActionResult> AddDocToProject([FromBody] AddDocToProjectModel model)
        {
            return await _documentLogic.AddDocToProject(model);
        }
        #endregion

        #region AddDocToMission
        /// <summary>
        /// Adds a document to a mission.
        /// </summary>
        [HttpPost("add-doc-to-mission")]
        public async Task<IActionResult> AddDocToMission([FromBody] AddDocToMissionModel model)
        {
            return await _documentLogic.AddDocToMission(model);
        }
        #endregion

        #region UpdateDoc
        /// <summary>
        /// Updates an existing document.
        /// </summary>
        [HttpPut("update-doc")]
        public async Task<IActionResult> UpdateDoc([FromBody] UpdateDocModel model)
        {
            return await _documentLogic.UpdateDoc(model);
        }
        #endregion

        #region DeleteDoc
        /// <summary>
        /// Deletes a document.
        /// </summary>
        [HttpDelete("delete-doc")]
        public async Task<IActionResult> DeleteDoc([FromBody] DeleteDocModel model)
        {
            return await _documentLogic.DeleteDoc(model);
        }
        #endregion
    }
}
