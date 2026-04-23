using back_end.BLL.DTOs;
namespace back_end.BLL.Services
{
    public interface IOrderService
    {
        List<OrderDto> GetAll();
        List<OrderDto> GetByUserId(int userId);
        OrderDto? GetById(int id);
        OrderDto Create(int userId, CreateOrderDto dto);
        OrderDto? UpdateStatus(int id, string status);
        bool Delete(int id);
        object GetStats();
    }
}