namespace ClickExpress.Domain.Models.Order
{
    public class UpdateOrderDTO
    {
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
    }
}
