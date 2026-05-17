using ClickExpress.Domain.Entities.User;

namespace ClickExpress.Domain.Entities.Cart
{
    public class CartData
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public UserData User { get; set; } = null!;
        public List<CartItemData> Items { get; set; } = new();
    }
}
