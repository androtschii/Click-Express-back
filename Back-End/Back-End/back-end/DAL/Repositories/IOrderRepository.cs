using back_end.Domain;

namespace back_end.DAL.Repositories
{
    public interface IOrderRepository
    {
        List<Order> GetAll();
        List<Order> GetByUserId(int userId);
        Order? GetById(int id);
        Order Create(Order order);
        Order? UpdateStatus(int id, string status);
        bool Delete(int id);
    }
}