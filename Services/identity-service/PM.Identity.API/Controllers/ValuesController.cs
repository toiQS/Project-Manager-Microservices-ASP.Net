using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PM.Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        public ValuesController() { }
        [HttpGet("demo")]
        public IActionResult Get()
        {
            return Ok(new { message = "Hello from Identity API" });
        }
    }
}
