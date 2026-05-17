using ClickExpress.Domain.Entities.Product;

namespace ClickExpress.Domain.Entities.Order
{
    public class OrderItemData
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public OrderData Order { get; set; } = null!;
        public ProductData Product { get; set; } = null!;
    }
}
