namespace ClickExpress.Domain.Models.Cart
{
    public class CartDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CartItemDTO> Items { get; set; } = new();
    }
}
