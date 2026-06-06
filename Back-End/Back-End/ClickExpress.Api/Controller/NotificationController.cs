using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.Domain.Models.Notification;
using ClickExpress.Domain.Models.Base;
using Microsoft.AspNetCore.Http;

namespace ClickExpress.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationActions _actions;

        public NotificationController(INotificationActions actions)
        {
            _actions = actions;
        }

        private int UserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        [HttpGet]
        [ProducesResponseType(typeof(List<NotificationDTO>), StatusCodes.Status200OK)]
        public IActionResult GetAll() => Ok(_actions.GetAllForUserAction(UserId));

        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<NotificationDTO>), StatusCodes.Status200OK)]
        public IActionResult GetPaged(
            [FromQuery] bool? unreadOnly,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
            => Ok(_actions.GetNotificationsPagedAction(UserId, unreadOnly, page, pageSize));

        [HttpGet("unread/count")]
        public IActionResult UnreadCount() => Ok(new { count = _actions.GetUnreadCountAction(UserId) });

        [HttpPatch("{id}/read")]
        public IActionResult MarkRead(int id)
        {
            var result = _actions.MarkReadAction(id, UserId);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpPatch("read-all")]
        public IActionResult MarkAllRead()
        {
            var result = _actions.MarkAllReadAction(UserId);
            return Ok(new { message = result.Message });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _actions.DeleteAction(id, UserId);
            if (!result.IsSuccess) return NotFound(new { message = result.Message });
            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Send([FromBody] CreateNotificationDTO dto)
        {
            var result = _actions.SendNotificationAction(dto);
            if (!result.IsSuccess) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }
    }
}
