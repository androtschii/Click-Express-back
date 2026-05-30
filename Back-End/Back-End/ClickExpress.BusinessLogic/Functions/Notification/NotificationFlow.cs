using ClickExpress.BusinessLogic.Core.Notification;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.Domain.Models.Notification;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Functions.Notification
{
    public class NotificationFlow : NotificationActions, INotificationActions
    {
        public List<NotificationDTO> GetAllForUserAction(int userId) => ExecuteGetAllForUserAction(userId);
        public int GetUnreadCountAction(int userId) => ExecuteGetUnreadCountAction(userId);
        public ResponseMsg MarkReadAction(int id, int userId) => ExecuteMarkReadAction(id, userId);
        public ResponseMsg MarkAllReadAction(int userId) => ExecuteMarkAllReadAction(userId);
        public ResponseMsg DeleteAction(int id, int userId) => ExecuteDeleteAction(id, userId);
        public ResponseMsg SendNotificationAction(CreateNotificationDTO dto) => ExecuteSendNotificationAction(dto);
        public PagedResult<NotificationDTO> GetNotificationsPagedAction(int userId, bool? unreadOnly, int page, int pageSize) => ExecuteGetNotificationsPagedAction(userId, unreadOnly, page, pageSize);
    }
}
