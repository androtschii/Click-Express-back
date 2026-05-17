using Microsoft.EntityFrameworkCore;
using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Entities.Cart;
using ClickExpress.Domain.Models.Cart;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Core.Cart
{
    public class CartActions
    {
        protected CartDTO ExecuteGetOrCreateCartAction(int userId)
        {
            using (var db = new OrderContext())
            {
                var cart = db.Carts.Include(c => c.Items).ThenInclude(i => i.Product)
                    .FirstOrDefault(c => c.UserId == userId);

                if (cart == null)
                {
                    cart = new CartData { UserId = userId, CreatedAt = DateTime.UtcNow };
                    db.Carts.Add(cart);
                    db.SaveChanges();
                }

                return MapToDTO(cart);
            }
        }

        protected ResponseAction ExecuteAddCartItemAction(int userId, int productId, int quantity)
        {
            if (quantity < 1)
                return new ResponseAction { IsSuccess = false, Message = "Quantity must be at least 1!" };

            using (var db = new OrderContext())
            {
                var product = db.Products.Find(productId);
                if (product == null)
                    return new ResponseAction { IsSuccess = false, Message = "Product not found!" };

                var cart = db.Carts.Include(c => c.Items).FirstOrDefault(c => c.UserId == userId);
                if (cart == null)
                {
                    cart = new CartData { UserId = userId, CreatedAt = DateTime.UtcNow };
                    db.Carts.Add(cart);
                    db.SaveChanges();
                }

                var existing = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (existing != null)
                    existing.Quantity += quantity;
                else
                    cart.Items.Add(new CartItemData { CartId = cart.Id, ProductId = productId, Quantity = quantity });

                db.SaveChanges();
                return new ResponseAction { IsSuccess = true, Message = "Item added!", Id = cart.Id };
            }
        }

        protected ResponseMsg ExecuteRemoveCartItemAction(int userId, int itemId)
        {
            using (var db = new OrderContext())
            {
                var cart = db.Carts.Include(c => c.Items).FirstOrDefault(c => c.UserId == userId);
                if (cart == null)
                    return new ResponseMsg { IsSuccess = false, Message = "Cart not found!" };

                var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
                if (item == null)
                    return new ResponseMsg { IsSuccess = false, Message = "Item not found in cart!" };

                db.CartItems.Remove(item);
                db.SaveChanges();
                return new ResponseMsg { IsSuccess = true, Message = "Item removed!" };
            }
        }

        protected ResponseMsg ExecuteClearCartAction(int userId)
        {
            using (var db = new OrderContext())
            {
                var cart = db.Carts.Include(c => c.Items).FirstOrDefault(c => c.UserId == userId);
                if (cart == null)
                    return new ResponseMsg { IsSuccess = true, Message = "Cart already empty!" };

                db.CartItems.RemoveRange(cart.Items);
                db.SaveChanges();
                return new ResponseMsg { IsSuccess = true, Message = "Cart cleared!" };
            }
        }

        private static CartDTO MapToDTO(CartData cart) => new CartDTO
        {
            Id = cart.Id, UserId = cart.UserId, CreatedAt = cart.CreatedAt,
            Items = cart.Items.Select(i => new CartItemDTO
            {
                Id = i.Id, ProductId = i.ProductId, Quantity = i.Quantity,
                ProductName = i.Product?.Name ?? string.Empty,
                Price = i.Product?.Price ?? 0,
                Total = (i.Product?.Price ?? 0) * i.Quantity
            }).ToList()
        };
    }
}
