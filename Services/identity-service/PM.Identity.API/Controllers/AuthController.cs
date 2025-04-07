using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PM.Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpGet("demo")]
        public IActionResult Get()
        {
            return Ok("AuthController is working");
        }
    }
}
