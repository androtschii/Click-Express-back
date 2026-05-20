using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClickExpress.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get() => Ok(new { status = "ok", timestamp = DateTime.UtcNow });

        [HttpGet("ping")]
        [AllowAnonymous]
        public IActionResult Ping() => Ok(new { status = "ok", timestamp = DateTime.UtcNow });
    }
}
