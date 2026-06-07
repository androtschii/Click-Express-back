using Microsoft.EntityFrameworkCore;
using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Entities.Order;
using ClickExpress.Domain.Models.Order;
using ClickExpress.Domain.Models.Base;
using ClickExpress.BusinessLogic.Helpers;

namespace ClickExpress.BusinessLogic.Core.Order
{
    public class OrderActions
    {
        protected List<OrderDTO> ExecuteGetAllOrdersAction()
        {
            using var db = new OrderContext();
            return db.Orders
                .AsNoTracking()
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDTO
                {
                    Id = o.Id, UserId = o.UserId, ProductId = o.ProductId,
                    ProductName = o.Product != null ? o.Product.Name : string.Empty,
                    Status = o.Status, CreatedAt = o.CreatedAt, Notes = o.Notes,
                    PickupAddress = o.PickupAddress, DeliveryAddress = o.DeliveryAddress,
                    PickupDate = o.PickupDate, DeliveryDate = o.DeliveryDate,
                    VehicleId = o.VehicleId, VehicleModel = o.Vehicle != null ? o.Vehicle.Model : null,
                    DriverId = o.DriverId, DriverName = o.Driver != null ? o.Driver.FullName : null,
                    TotalPrice = o.TotalPrice, CurrentLocation = o.CurrentLocation,
                    EstimatedArrival = o.EstimatedArrival, TrackingCode = o.TrackingCode
                })
                .ToList();
        }

        protected List<OrderDTO> ExecuteGetOrdersByUserIdAction(int userId)
        {
            using var db = new OrderContext();
            return db.Orders
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDTO
                {
                    Id = o.Id, UserId = o.UserId, ProductId = o.ProductId,
                    ProductName = o.Product != null ? o.Product.Name : string.Empty,
                    Status = o.Status, CreatedAt = o.CreatedAt, Notes = o.Notes,
                    PickupAddress = o.PickupAddress, DeliveryAddress = o.DeliveryAddress,
                    PickupDate = o.PickupDate, DeliveryDate = o.DeliveryDate,
                    VehicleId = o.VehicleId, VehicleModel = o.Vehicle != null ? o.Vehicle.Model : null,
                    DriverId = o.DriverId, DriverName = o.Driver != null ? o.Driver.FullName : null,
                    TotalPrice = o.TotalPrice, CurrentLocation = o.CurrentLocation,
                    EstimatedArrival = o.EstimatedArrival, TrackingCode = o.TrackingCode
                })
                .ToList();
        }

        protected OrderDTO? ExecuteGetOrderByIdAction(int id)
        {
            using var db = new OrderContext();
            return db.Orders
                .AsNoTracking()
                .Where(o => o.Id == id)
                .Select(o => new OrderDTO
                {
                    Id = o.Id, UserId = o.UserId, ProductId = o.ProductId,
                    ProductName = o.Product != null ? o.Product.Name : string.Empty,
                    Status = o.Status, CreatedAt = o.CreatedAt, Notes = o.Notes,
                    PickupAddress = o.PickupAddress, DeliveryAddress = o.DeliveryAddress,
                    PickupDate = o.PickupDate, DeliveryDate = o.DeliveryDate,
                    VehicleId = o.VehicleId, VehicleModel = o.Vehicle != null ? o.Vehicle.Model : null,
                    DriverId = o.DriverId, DriverName = o.Driver != null ? o.Driver.FullName : null,
                    TotalPrice = o.TotalPrice, CurrentLocation = o.CurrentLocation,
                    EstimatedArrival = o.EstimatedArrival, TrackingCode = o.TrackingCode
                })
                .FirstOrDefault();
        }

        protected PagedResult<OrderDTO> ExecuteGetOrdersPagedAction(
            string? status, int? userId, string? search, int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize is < 1 or > 100 ? 25 : pageSize;

            using var db = new OrderContext();

            var query = db.Orders.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(o => o.Status == status);

            if (userId.HasValue)
                query = query.Where(o => o.UserId == userId.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                query = query.Where(o =>
                    (o.PickupAddress != null && o.PickupAddress.ToLower().Contains(s)) ||
                    (o.DeliveryAddress != null && o.DeliveryAddress.ToLower().Contains(s)) ||
                    (o.Notes != null && o.Notes.ToLower().Contains(s)));
            }

            var total = query.Count();
            var items = query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderDTO
                {
                    Id = o.Id, UserId = o.UserId, ProductId = o.ProductId,
                    ProductName = o.Product != null ? o.Product.Name : string.Empty,
                    Status = o.Status, CreatedAt = o.CreatedAt, Notes = o.Notes,
                    PickupAddress = o.PickupAddress, DeliveryAddress = o.DeliveryAddress,
                    PickupDate = o.PickupDate, DeliveryDate = o.DeliveryDate,
                    VehicleId = o.VehicleId, VehicleModel = o.Vehicle != null ? o.Vehicle.Model : null,
                    DriverId = o.DriverId, DriverName = o.Driver != null ? o.Driver.FullName : null,
                    TotalPrice = o.TotalPrice, CurrentLocation = o.CurrentLocation,
                    EstimatedArrival = o.EstimatedArrival, TrackingCode = o.TrackingCode
                })
                .ToList();

            return new PagedResult<OrderDTO>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        protected OrderDTO? ExecuteGetOrderByTrackingCodeAction(string code)
        {
            using var db = new OrderContext();
            return db.Orders
                .AsNoTracking()
                .Where(o => o.TrackingCode == code)
                .Select(o => new OrderDTO
                {
                    Id = o.Id, UserId = o.UserId, ProductId = o.ProductId,
                    ProductName = o.Product != null ? o.Product.Name : string.Empty,
                    Status = o.Status, CreatedAt = o.CreatedAt, Notes = o.Notes,
                    PickupAddress = o.PickupAddress, DeliveryAddress = o.DeliveryAddress,
                    PickupDate = o.PickupDate, DeliveryDate = o.DeliveryDate,
                    VehicleId = o.VehicleId, VehicleModel = o.Vehicle != null ? o.Vehicle.Model : null,
                    DriverId = o.DriverId, DriverName = o.Driver != null ? o.Driver.FullName : null,
                    TotalPrice = o.TotalPrice, CurrentLocation = o.CurrentLocation,
                    EstimatedArrival = o.EstimatedArrival, TrackingCode = o.TrackingCode
                })
                .FirstOrDefault();
        }

        protected ResponseAction ExecuteCreateOrderAction(int userId, CreateOrderDTO dto)
        {
            using var db = new OrderContext();

            var order = new OrderData
            {
                UserId = userId, ProductId = dto.ProductId, Status = "Pending",
                CreatedAt = DateTime.UtcNow, Notes = dto.Notes,
                PickupAddress = dto.PickupAddress, DeliveryAddress = dto.DeliveryAddress,
                PickupDate = dto.PickupDate, DeliveryDate = dto.DeliveryDate,
                VehicleId = dto.VehicleId, DriverId = dto.DriverId, TotalPrice = dto.TotalPrice,
                TrackingCode = TrackingCodeHelper.Generate()
            };
            db.Orders.Add(order);
            db.SaveChanges();

            db.OrderStatusHistories.Add(new OrderStatusHistoryData
            {
                OrderId = order.Id, Status = "Pending", Timestamp = DateTime.UtcNow
            });
            db.SaveChanges();

            return new ResponseAction { IsSuccess = true, Message = "Order created!", Id = order.Id };
        }

        protected ResponseMsg ExecuteUpdateOrderStatusAction(int id, string status)
        {
            using var db = new OrderContext();

            var order = db.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
                return new ResponseMsg { IsSuccess = false, Message = "Order not found!" };

            order.Status = status;
            db.OrderStatusHistories.Add(new OrderStatusHistoryData
            {
                OrderId = id, Status = status, Timestamp = DateTime.UtcNow,
                Location = order.CurrentLocation
            });
            db.SaveChanges();
            return new ResponseMsg { IsSuccess = true, Message = "Status updated!" };
        }

        protected ResponseMsg ExecuteUpdateOrderAction(int id, UpdateOrderDTO dto)
        {
            using var db = new OrderContext();

            var order = db.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
                return new ResponseMsg { IsSuccess = false, Message = "Order not found!" };

            order.Notes = dto.Notes; order.PickupAddress = dto.PickupAddress;
            order.DeliveryAddress = dto.DeliveryAddress; order.PickupDate = dto.PickupDate;
            order.DeliveryDate = dto.DeliveryDate; order.VehicleId = dto.VehicleId;
            order.DriverId = dto.DriverId; order.TotalPrice = dto.TotalPrice;
            order.CurrentLocation = dto.CurrentLocation; order.EstimatedArrival = dto.EstimatedArrival;
            db.SaveChanges();
            return new ResponseMsg { IsSuccess = true, Message = "Order updated!" };
        }

        protected ResponseMsg ExecuteDeleteOrderAction(int id)
        {
            using var db = new OrderContext();

            var order = db.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
                return new ResponseMsg { IsSuccess = false, Message = "Order not found!" };

            db.Orders.Remove(order);
            db.SaveChanges();
            return new ResponseMsg { IsSuccess = true, Message = "Order deleted!" };
        }

        protected List<OrderStatusHistoryDTO> ExecuteGetOrderTrackingAction(int orderId)
        {
            using var db = new OrderContext();
            return db.OrderStatusHistories
                .AsNoTracking()
                .Where(h => h.OrderId == orderId)
                .OrderBy(h => h.Timestamp)
                .Select(h => new OrderStatusHistoryDTO
                {
                    Id = h.Id, OrderId = h.OrderId, Status = h.Status,
                    Timestamp = h.Timestamp, Location = h.Location, Note = h.Note
                })
                .ToList();
        }

        protected object ExecuteGetOrderStatsAction()
        {
            using var db = new OrderContext();
            return new
            {
                Total      = db.Orders.Count(),
                Pending    = db.Orders.Count(o => o.Status == "Pending"),
                Approved   = db.Orders.Count(o => o.Status == "Approved"),
                Cancelled  = db.Orders.Count(o => o.Status == "Cancelled"),
                TotalRevenue = db.Orders
                    .Where(o => o.Status == "Approved")
                    .Sum(o => (decimal?)(o.TotalPrice ?? 0)) ?? 0
            };
        }
    }
}
