namespace ClickExpress.Domain.Models.Cart
{
    public class CartDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CartItemDTO> Items { get; set; } = new();
    }

    public class CartItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }

    public class AddCartItemDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
