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
        private IQueryable<Order> WithIncludes()
            => _db.Orders
                .Include(o => o.Product)
                .Include(o => o.User)
                .Include(o => o.Vehicle)
                .Include(o => o.Driver);

        public List<Order> GetAll()
            => WithIncludes().ToList();
        public List<Order> GetByUserId(int userId)
            => WithIncludes().Where(o => o.UserId == userId).ToList();
        public Order? GetById(int id)
            => WithIncludes().FirstOrDefault(o => o.Id == id);
        public Order Create(Order order)
        {
            _db.Orders.Add(order);
            _db.SaveChanges();
            return WithIncludes().First(o => o.Id == order.Id);
        }
        public Order? UpdateStatus(int id, string status)
        {
            var order = _db.Orders.Find(id);
            if (order == null) return null;
            order.Status = status;
            _db.SaveChanges();
            return WithIncludes().FirstOrDefault(o => o.Id == id);
        }

        public Order? Update(int id, Action<Order> apply)
        {
            var order = _db.Orders.Find(id);
            if (order == null) return null;
            apply(order);
            _db.SaveChanges();
            return WithIncludes().FirstOrDefault(o => o.Id == id);
        }
        public bool Delete(int id)
        {
            var order = _db.Orders.Find(id);
            if (order == null) return false;
            _db.Orders.Remove(order);
            _db.SaveChanges();
            return true;
        }

        public List<OrderStatusHistory> GetHistory(int orderId)
            => _db.OrderStatusHistories
                .Where(h => h.OrderId == orderId)
                .OrderBy(h => h.Timestamp)
                .ToList();

        public OrderStatusHistory AddHistory(OrderStatusHistory entry)
        {
            _db.OrderStatusHistories.Add(entry);
            _db.SaveChanges();
            return entry;
        }
    }
}