using ClickExpress.Domain.Entities.User;
using ClickExpress.Domain.Entities.Product;

namespace ClickExpress.Domain.Entities.SavedLoad
{
    public class SavedLoadData
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public UserData User { get; set; } = null!;
        public ProductData Product { get; set; } = null!;
    }
}
