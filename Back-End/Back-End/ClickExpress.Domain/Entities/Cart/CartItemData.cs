using ClickExpress.Domain.Entities.Product;

namespace ClickExpress.Domain.Entities.Cart
{
    public class CartItemData
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public CartData Cart { get; set; } = null!;
        public ProductData Product { get; set; } = null!;
    }
}
