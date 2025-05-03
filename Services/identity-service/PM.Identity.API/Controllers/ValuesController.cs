using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PM.Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;

        public ValuesController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = new HttpClient();
        }
        [HttpGet("demo-2")]
        public IActionResult demo2()
        {
            var user = _httpContextAccessor.HttpContext.User;
            if(user.Identity.IsAuthenticated)
            {
                return Ok(new { message = "Hello from Identity API" });
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpGet("demo")]
        public IActionResult Get()
        {
            return Ok(new { message = "Hello from Identity API" });
        }
    }
}
