using Microsoft.EntityFrameworkCore;
using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Entities.Notification;
using ClickExpress.Domain.Models.Notification;
using ClickExpress.Domain.Models.Base;
using ClickExpress.Domain.Entities.User;

namespace ClickExpress.BusinessLogic.Core.Notification
{
    public class NotificationActions
    {
        protected List<NotificationDTO> ExecuteGetAllForUserAction(int userId)
        {
            using var db = new OrderContext();
            return db.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDTO
                {
                    Id = n.Id, Title = n.Title, Body = n.Body,
                    Type = n.Type, IsRead = n.IsRead, CreatedAt = n.CreatedAt,
                })
                .ToList();
        }

        protected PagedResult<NotificationDTO> ExecuteGetNotificationsPagedAction(int userId, bool? unreadOnly, int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;

            using var db = new OrderContext();

            var query = db.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId
                         && (!unreadOnly.HasValue || !unreadOnly.Value || !n.IsRead));

            var total = query.Count();
            var items = query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new NotificationDTO
                {
                    Id = n.Id, Title = n.Title, Body = n.Body,
                    Type = n.Type, IsRead = n.IsRead, CreatedAt = n.CreatedAt,
                })
                .ToList();

            return new PagedResult<NotificationDTO>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        protected int ExecuteGetUnreadCountAction(int userId)
        {
            using var db = new OrderContext();
            return db.Notifications.AsNoTracking().Count(n => n.UserId == userId && !n.IsRead);
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

        protected ResponseMsg ExecuteSendNotificationAction(CreateNotificationDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Body))
                return new ResponseMsg { IsSuccess = false, Message = "Title and body are required" };

            using var orderDb = new OrderContext();

            if (dto.UserId.HasValue)
            {
                orderDb.Notifications.Add(new NotificationData
                {
                    UserId = dto.UserId.Value,
                    Title = dto.Title,
                    Body = dto.Body,
                    Type = dto.Type,
                    CreatedAt = DateTime.UtcNow,
                });
                orderDb.SaveChanges();
                return new ResponseMsg { IsSuccess = true, Message = "Notification sent" };
            }

            List<int> userIds;
            using (var userDb = new UserContext())
                userIds = userDb.Users.Where(u => u.IsActive).Select(u => u.Id).ToList();

            foreach (var uid in userIds)
            {
                orderDb.Notifications.Add(new NotificationData
                {
                    UserId = uid,
                    Title = dto.Title,
                    Body = dto.Body,
                    Type = dto.Type,
                    CreatedAt = DateTime.UtcNow,
                });
            }
            orderDb.SaveChanges();
            return new ResponseMsg { IsSuccess = true, Message = $"Sent to {userIds.Count} users" };
        }
    }
}
