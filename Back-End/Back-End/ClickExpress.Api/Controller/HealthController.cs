using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ClickExpress.Api.Controller
{
    /// <summary>Simple liveness check endpoints used by load balancers and uptime monitors.</summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        /// <summary>Returns 200 OK with current UTC timestamp. Use for uptime monitoring.</summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get() => Ok(new { status = "ok", timestamp = DateTime.UtcNow });

        /// <summary>Lightweight ping endpoint — no DB access, fastest possible response.</summary>
        /// <remarks>Use /health/ready (via ASP.NET health checks) for DB connectivity verification.</remarks>
        [HttpGet("ping")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Ping() => Ok(new { status = "ok", timestamp = DateTime.UtcNow });
    }
}
