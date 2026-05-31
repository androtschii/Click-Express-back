using ClickExpress.Domain.Models.Order;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Interfaces
{
    public interface IOrderActions
    {
        List<OrderDTO> GetAllOrdersAction();
        List<OrderDTO> GetOrdersByUserIdAction(int userId);
        OrderDTO? GetOrderByIdAction(int id);
        ResponseAction ResponseCreateOrderAction(int userId, CreateOrderDTO dto);
        ResponseMsg ResponseUpdateOrderStatusAction(int id, string status);
        ResponseMsg ResponseUpdateOrderAction(int id, UpdateOrderDTO dto);
        ResponseMsg ResponseDeleteOrderAction(int id);
        List<OrderStatusHistoryDTO> GetOrderTrackingAction(int orderId);
        object GetOrderStatsAction();
        PagedResult<OrderDTO> GetOrdersPagedAction(string? status, int? userId, string? search, int page, int pageSize);
        OrderDTO? GetOrderByTrackingCodeAction(string code);
    }
}
