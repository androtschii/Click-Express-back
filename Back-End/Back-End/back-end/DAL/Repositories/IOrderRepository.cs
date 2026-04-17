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
Создать новый файл back-end/DAL/Repositories/OrderRepository.cs:


using back_end.Domain;
using Microsoft.EntityFrameworkCore;

namespace back_end.DAL.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _db;

        public OrderRepository(AppDbContext db)
        {
            _db = db;
        }

        public List<Order> GetAll()
            => _db.Orders.Include(o => o.Product).Include(o => o.User).ToList();

        public List<Order> GetByUserId(int userId)
            => _db.Orders.Include(o => o.Product).Where(o => o.UserId == userId).ToList();

        public Order? GetById(int id)
            => _db.Orders.Include(o => o.Product).FirstOrDefault(o => o.Id == id);

        public Order Create(Order order)
        {
            _db.Orders.Add(order);
            _db.SaveChanges();
            return _db.Orders.Include(o => o.Product).First(o => o.Id == order.Id);
        }

        public Order? UpdateStatus(int id, string status)
        {
            var order = _db.Orders.Find(id);
            if (order == null) return null;
            order.Status = status;
            _db.SaveChanges();
            return order;
        }

        public bool Delete(int id)
        {
            var order = _db.Orders.Find(id);
            if (order == null) return false;
            _db.Orders.Remove(order);
            _db.SaveChanges();
            return true;
        }
    }
}
