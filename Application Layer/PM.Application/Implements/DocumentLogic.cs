using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PM.Application.Interfaces;
using PM.Application.Models.docs;
using PM.Domain.Interfaces.Services;
using PM.Domain.Models.documents;

namespace PM.Application.Implements
{
    public class DocumentLogic : ControllerBase, IDocumentLogic
    {
        private readonly IDocumentServices _docServices;
        private readonly ILogger<DocumentLogic> _logger;

        /// <summary>
        /// Constructor for DocumentLogic.
        /// Initializes document services and logger.
        /// </summary>
        public DocumentLogic(IDocumentServices docServices, ILogger<DocumentLogic> logger)
        {
            _docServices = docServices;
            _logger = logger;
        }

        #region Document Retrieval

        /// <summary>
        /// Retrieves all documents.
        /// </summary>
        /// <returns>List of documents.</returns>
        public async Task<IActionResult> GetDocs()
        {
            try
            {
                var docResponse = await _docServices.GetDocs();
                if (!docResponse.Status) return BadRequest(docResponse.Message);
                return Ok(docResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving documents.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Retrieves documents within a specific project.
        /// </summary>
        /// <param name="projectId">Project identifier.</param>
        /// <returns>List of documents in the project.</returns>
        public async Task<IActionResult> GetDocsInProject(string projectId)
        {
            try
            {
                var docResponse = await _docServices.GetDocsInProject(projectId);
                if (!docResponse.Status) return BadRequest(docResponse.Message);
                return Ok(docResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving documents in project.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Retrieves documents within a specific plan.
        /// </summary>
        /// <param name="planId">Plan identifier.</param>
        /// <returns>List of documents in the plan.</returns>
        public async Task<IActionResult> GetDocsInPlan(string planId)
        {
            try
            {
                var docResponse = await _docServices.GetDocsInPlan(planId);
                if (!docResponse.Status) return BadRequest(docResponse.Message);
                return Ok(docResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving documents in plan.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Retrieves documents within a specific mission.
        /// </summary>
        /// <param name="missionId">Mission identifier.</param>
        /// <returns>List of documents in the mission.</returns>
        public async Task<IActionResult> GetDocsInMission(string missionId)
        {
            try
            {
                var docResponse = await _docServices.GetDocsInMission(missionId);
                if (!docResponse.Status) return BadRequest(docResponse.Message);
                return Ok(docResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving documents in mission.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Retrieves a specific document.
        /// </summary>
        /// <param name="docId">Document identifier.</param>
        /// <returns>The requested document.</returns>
        public async Task<IActionResult> GetDoc(string docId)
        {
            try
            {
                var docResponse = await _docServices.GetDoc(docId);
                if (!docResponse.Status) return BadRequest(docResponse.Message);
                return Ok(docResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving document.");
                return StatusCode(500, "Internal server error");
            }
        }

        #endregion

        #region Document Management

        /// <summary>
        /// Adds a document to a project.
        /// </summary>
        public async Task<IActionResult> AddDocToProject(AddDocToProjectModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid model state.");
            try
            {
                var addDoc = new AddDoc { Description = model.Description, Name = model.Name, Path = model.Path };
                var docResponse = await _docServices.AddDocToProject(model.MemberId, model.ProjectId, addDoc);
                if (!docResponse.Status) return BadRequest(docResponse.Message);
                return Ok(docResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding document to project.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Adds a document to a mission.
        /// </summary>
        public async Task<IActionResult> AddDocToMission(AddDocToMissionModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid model state.");
            try
            {
                var addDoc = new AddDoc { Description = model.Description, Name = model.Name, Path = model.Path };
                var docResponse = await _docServices.AddDocToMission(model.MemberId, model.MissionId, addDoc);
                if (!docResponse.Status) return BadRequest(docResponse.Message);
                return Ok(docResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding document to mission.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates an existing document.
        /// </summary>
        public async Task<IActionResult> UpdateDoc(UpdateDocModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid model state.");
            try
            {
                var updateDoc = new UpdateDoc { Description = model.Description, Name = model.Name, Path = model.Path };
                var docResponse = await _docServices.UpdateDoc(model.MemberId, model.DocId, updateDoc);
                if (!docResponse.Status) return BadRequest(docResponse.Message);
                return Ok(docResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating document.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a document.
        /// </summary>
        public async Task<IActionResult> DeleteDoc(DeleteDocModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid model state.");
            try
            {
                var docResponse = await _docServices.DeleteDoc(model.MemberId, model.DocId);
                if (!docResponse.Status) return BadRequest(docResponse.Message);
                return Ok(docResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting document.");
                return StatusCode(500, "Internal server error");
            }
        }

        #endregion
    }
}
