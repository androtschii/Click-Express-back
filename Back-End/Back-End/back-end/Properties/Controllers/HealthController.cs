using Microsoft.AspNetCore.Mvc;
using System;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new
            {
                message = "pong",
                timestamp = DateTime.Now,
                status = "API is running healthy"
            });
        }
    }
}