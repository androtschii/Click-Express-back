namespace ClickExpress.Domain.Models.Order
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? Notes { get; set; }
        public string? PickupAddress { get; set; }
        public string? DeliveryAddress { get; set; }
        public DateTime? PickupDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int? VehicleId { get; set; }
        public string? VehicleModel { get; set; }
        public int? DriverId { get; set; }
        public string? DriverName { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? CurrentLocation { get; set; }
        public DateTime? EstimatedArrival { get; set; }
        public string? TrackingCode { get; set; }
    }
}
