using Microsoft.EntityFrameworkCore;
using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Entities.Order;
using ClickExpress.Domain.Models.Order;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Core.Order
{
    public class OrderActions
    {
        protected List<OrderDTO> ExecuteGetAllOrdersAction()
        {
            using (var db = new OrderContext())
            {
                return db.Orders
                    .Include(o => o.Product).Include(o => o.Vehicle).Include(o => o.Driver)
                    .Select(o => MapToDTO(o)).ToList();
            }
        }

        protected List<OrderDTO> ExecuteGetOrdersByUserIdAction(int userId)
        {
            using (var db = new OrderContext())
            {
                return db.Orders.Where(o => o.UserId == userId)
                    .Include(o => o.Product).Include(o => o.Vehicle).Include(o => o.Driver)
                    .Select(o => MapToDTO(o)).ToList();
            }
        }

        protected OrderDTO? ExecuteGetOrderByIdAction(int id)
        {
            using (var db = new OrderContext())
            {
                var o = db.Orders.Where(o => o.Id == id)
                    .Include(o => o.Product).Include(o => o.Vehicle).Include(o => o.Driver)
                    .FirstOrDefault();
                return o == null ? null : MapToDTO(o);
            }
        }

        protected ResponseAction ExecuteCreateOrderAction(int userId, CreateOrderDTO dto)
        {
            using (var db = new OrderContext())
            {
                var order = new OrderData
                {
                    UserId = userId, ProductId = dto.ProductId, Status = "Pending",
                    CreatedAt = DateTime.UtcNow, Notes = dto.Notes,
                    PickupAddress = dto.PickupAddress, DeliveryAddress = dto.DeliveryAddress,
                    PickupDate = dto.PickupDate, DeliveryDate = dto.DeliveryDate,
                    VehicleId = dto.VehicleId, DriverId = dto.DriverId, TotalPrice = dto.TotalPrice
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
        }

        protected ResponseMsg ExecuteUpdateOrderStatusAction(int id, string status)
        {
            using (var db = new OrderContext())
            {
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
        }

        protected ResponseMsg ExecuteUpdateOrderAction(int id, UpdateOrderDTO dto)
        {
            using (var db = new OrderContext())
            {
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
        }

        protected ResponseMsg ExecuteDeleteOrderAction(int id)
        {
            using (var db = new OrderContext())
            {
                var order = db.Orders.FirstOrDefault(o => o.Id == id);
                if (order == null)
                    return new ResponseMsg { IsSuccess = false, Message = "Order not found!" };

                db.Orders.Remove(order);
                db.SaveChanges();
                return new ResponseMsg { IsSuccess = true, Message = "Order deleted!" };
            }
        }

        protected List<OrderStatusHistoryDTO> ExecuteGetOrderTrackingAction(int orderId)
        {
            using (var db = new OrderContext())
            {
                return db.OrderStatusHistories.Where(h => h.OrderId == orderId)
                    .OrderBy(h => h.Timestamp)
                    .Select(h => new OrderStatusHistoryDTO
                    {
                        Id = h.Id, OrderId = h.OrderId, Status = h.Status,
                        Timestamp = h.Timestamp, Location = h.Location, Note = h.Note
                    }).ToList();
            }
        }

        protected object ExecuteGetOrderStatsAction()
        {
            using (var db = new OrderContext())
            {
                var orders = db.Orders.Include(o => o.Product).ToList();
                return new
                {
                    Total = orders.Count,
                    Pending = orders.Count(o => o.Status == "Pending"),
                    Approved = orders.Count(o => o.Status == "Approved"),
                    Cancelled = orders.Count(o => o.Status == "Cancelled"),
                    TotalRevenue = orders.Where(o => o.Status == "Approved").Sum(o => o.TotalPrice ?? o.Product?.Price ?? 0)
                };
            }
        }

        private static OrderDTO MapToDTO(OrderData o) => new OrderDTO
        {
            Id = o.Id, UserId = o.UserId, ProductId = o.ProductId,
            ProductName = o.Product?.Name ?? string.Empty, Status = o.Status,
            CreatedAt = o.CreatedAt, Notes = o.Notes, PickupAddress = o.PickupAddress,
            DeliveryAddress = o.DeliveryAddress, PickupDate = o.PickupDate, DeliveryDate = o.DeliveryDate,
            VehicleId = o.VehicleId, VehicleModel = o.Vehicle?.Model,
            DriverId = o.DriverId, DriverName = o.Driver?.FullName,
            TotalPrice = o.TotalPrice, CurrentLocation = o.CurrentLocation, EstimatedArrival = o.EstimatedArrival
        };
    }
}
