using Microsoft.EntityFrameworkCore;
using back_end.Domain;

namespace back_end.DAL.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _db;
        public CartRepository(AppDbContext db) => _db = db;

        public Cart GetOrCreate(int userId)
        {
            var cart = _db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart != null) return cart;

            cart = new Cart { UserId = userId };
            _db.Carts.Add(cart);
            _db.SaveChanges();
            return cart;
        }

        public CartItem? GetItem(int itemId)
            => _db.CartItems.Include(i => i.Cart).FirstOrDefault(i => i.Id == itemId);

        public void Save() => _db.SaveChanges();
    }
}
