using ClickExpress.Domain.Entities.User;
using ClickExpress.Domain.Entities.Product;
using ClickExpress.Domain.Entities.Vehicle;
using ClickExpress.Domain.Entities.Driver;

namespace ClickExpress.Domain.Entities.Order
{
    public class OrderData
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }

        public string? PickupAddress { get; set; }
        public string? DeliveryAddress { get; set; }
        public DateTime? PickupDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int? VehicleId { get; set; }
        public int? DriverId { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? CurrentLocation { get; set; }
        public DateTime? EstimatedArrival { get; set; }
        public string? TrackingCode { get; set; }

        public UserData User { get; set; } = null!;
        public ProductData Product { get; set; } = null!;
        public VehicleData? Vehicle { get; set; }
        public DriverData? Driver { get; set; }
    }
}
