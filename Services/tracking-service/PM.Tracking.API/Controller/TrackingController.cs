using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.Shared.Dtos.auths;
using PM.Shared.Dtos.tracking;
using PM.Tracking.Application.Interfaces;

namespace PM.Tracking.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackingController : ControllerBase
    {

        private readonly ITrackingHandle _trackingHandle;
        private readonly ILogger<TrackingController> _logger;
        public TrackingController(ITrackingHandle trackingHandle, ILogger<TrackingController> logger)
        {
            _trackingHandle = trackingHandle;
            _logger = logger;
        }
        [HttpPost("add-tracking-log")]
        public async Task<IActionResult> AddAsync(AddTrackingModel model)
        {
            _logger.LogInformation("Adding tracking activity log");
            var result = await _trackingHandle.AddHandle(model);
            if (result.Status != ResultStatus.Success)
            {
                _logger.LogError("Error when to do action");
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
