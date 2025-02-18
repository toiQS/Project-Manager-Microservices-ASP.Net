using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.Application.Interfaces;

namespace ManagerAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserLogic _userLogic;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserLogic userLogic, IHttpContextAccessor httpContextAccessor, ILogger<UserController> logger)
        {
            _userLogic = userLogic;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        [HttpGet("user-detail")]
        public async Task<IActionResult> GetUserDetail(string token)
        {
            try
            {
                //var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                var response = await _userLogic.DetailUser(token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
