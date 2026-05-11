using AutoMapper;
using Microsoft.EntityFrameworkCore;
using back_end.BLL.DTOs;
using back_end.DAL;
using back_end.DAL.Repositories;
using back_end.Domain;

namespace back_end.BLL.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepo;
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public CartService(ICartRepository cartRepo, AppDbContext db, IMapper mapper)
        {
            _cartRepo = cartRepo;
            _db = db;
            _mapper = mapper;
        }

        public CartDto GetOrCreate(int userId)
            => _mapper.Map<CartDto>(_cartRepo.GetOrCreate(userId));

        public CartDto AddItem(int userId, int productId, int quantity)
        {
            if (quantity < 1) throw new ArgumentException("Quantity must be >= 1");

            var product = _db.Products.Find(productId)
                ?? throw new KeyNotFoundException($"Product {productId} not found");

            var cart = _cartRepo.GetOrCreate(userId);
            var existing = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existing != null)
                existing.Quantity += quantity;
            else
                cart.Items.Add(new CartItem { ProductId = productId, Quantity = quantity });

            _cartRepo.Save();
            return _mapper.Map<CartDto>(_cartRepo.GetOrCreate(userId));
        }

        public void RemoveItem(int itemId)
        {
            var item = _cartRepo.GetItem(itemId)
                ?? throw new KeyNotFoundException($"CartItem {itemId} not found");
            _db.CartItems.Remove(item);
            _cartRepo.Save();
        }

        public void Clear(int userId)
        {
            var cart = _db.Carts.Include(c => c.Items).FirstOrDefault(c => c.UserId == userId);
            if (cart == null) return;
            _db.CartItems.RemoveRange(cart.Items);
            _cartRepo.Save();
        }
    }
}
