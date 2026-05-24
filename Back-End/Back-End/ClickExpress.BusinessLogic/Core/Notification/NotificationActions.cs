using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Entities.Notification;
using ClickExpress.Domain.Models.Notification;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Core.Notification
{
    public class NotificationActions
    {
        protected List<NotificationDTO> ExecuteGetAllForUserAction(int userId)
        {
            using var db = new OrderContext();
            return db.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDTO
                {
                    Id = n.Id, Title = n.Title, Body = n.Body,
                    Type = n.Type, IsRead = n.IsRead, CreatedAt = n.CreatedAt,
                })
                .ToList();
        }

        protected int ExecuteGetUnreadCountAction(int userId)
        {
            using var db = new OrderContext();
            return db.Notifications.Count(n => n.UserId == userId && !n.IsRead);
        }

        protected ResponseMsg ExecuteMarkReadAction(int id, int userId)
        {
            using var db = new OrderContext();
            var n = db.Notifications.FirstOrDefault(n => n.Id == id && n.UserId == userId);
            if (n == null) return new ResponseMsg { IsSuccess = false, Message = "Not found" };
            n.IsRead = true;
            db.SaveChanges();
            return new ResponseMsg { IsSuccess = true, Message = "Marked as read" };
        }

        protected ResponseMsg ExecuteMarkAllReadAction(int userId)
        {
            using var db = new OrderContext();
            var list = db.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToList();
            list.ForEach(n => n.IsRead = true);
            db.SaveChanges();
            return new ResponseMsg { IsSuccess = true, Message = $"{list.Count} marked as read" };
        }

        protected ResponseMsg ExecuteDeleteAction(int id, int userId)
        {
            using var db = new OrderContext();
            var n = db.Notifications.FirstOrDefault(n => n.Id == id && n.UserId == userId);
            if (n == null) return new ResponseMsg { IsSuccess = false, Message = "Not found" };
            db.Notifications.Remove(n);
            db.SaveChanges();
            return new ResponseMsg { IsSuccess = true, Message = "Deleted" };
        }
    }
}
