using ClickExpress.Domain.Models.Notification;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Interfaces
{
    public interface INotificationActions
    {
        List<NotificationDTO> GetAllForUserAction(int userId);
        int GetUnreadCountAction(int userId);
        ResponseMsg MarkReadAction(int id, int userId);
        ResponseMsg MarkAllReadAction(int userId);
        ResponseMsg DeleteAction(int id, int userId);
        ResponseMsg SendNotificationAction(CreateNotificationDTO dto);
        PagedResult<NotificationDTO> GetNotificationsPagedAction(int userId, bool? unreadOnly, int page, int pageSize);
    }
}
