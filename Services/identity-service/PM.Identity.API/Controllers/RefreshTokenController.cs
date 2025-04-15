using Microsoft.AspNetCore.Mvc;
using PM.Identity.Application.Interfaces;

namespace PM.Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefreshTokenController : ControllerBase
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ILogger<RefreshTokenController> _logger;
        public RefreshTokenController(IRefreshTokenService refreshTokenService, ILogger<RefreshTokenController> logger)
        {
            _refreshTokenService = refreshTokenService;
            _logger = logger;
        }
        
    }
}
