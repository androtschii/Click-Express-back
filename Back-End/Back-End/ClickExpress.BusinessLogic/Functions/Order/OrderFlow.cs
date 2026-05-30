using ClickExpress.BusinessLogic.Core.Order;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.Domain.Models.Order;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Functions.Order
{
    public class OrderFlow : OrderActions, IOrderActions
    {
        public List<OrderDTO> GetAllOrdersAction() => ExecuteGetAllOrdersAction();
        public List<OrderDTO> GetOrdersByUserIdAction(int userId) => ExecuteGetOrdersByUserIdAction(userId);
        public OrderDTO? GetOrderByIdAction(int id) => ExecuteGetOrderByIdAction(id);
        public ResponseAction ResponseCreateOrderAction(int userId, CreateOrderDTO dto) => ExecuteCreateOrderAction(userId, dto);
        public ResponseMsg ResponseUpdateOrderStatusAction(int id, string status) => ExecuteUpdateOrderStatusAction(id, status);
        public ResponseMsg ResponseUpdateOrderAction(int id, UpdateOrderDTO dto) => ExecuteUpdateOrderAction(id, dto);
        public ResponseMsg ResponseDeleteOrderAction(int id) => ExecuteDeleteOrderAction(id);
        public List<OrderStatusHistoryDTO> GetOrderTrackingAction(int orderId) => ExecuteGetOrderTrackingAction(orderId);
        public object GetOrderStatsAction() => ExecuteGetOrderStatsAction();
        public PagedResult<OrderDTO> GetOrdersPagedAction(string? status, int? userId, string? search, int page, int pageSize) => ExecuteGetOrdersPagedAction(status, userId, search, page, pageSize);
    }
}
