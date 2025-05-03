using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.Core.Application.Interfaces;

namespace PM.Core.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProjectHandle _projectHandle;
        
    }
}
