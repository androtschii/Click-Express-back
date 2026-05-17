using System.ComponentModel.DataAnnotations;

namespace ClickExpress.Domain.Models.Order
{
    public class CreateOrderDTO
    {
        [Required] public int ProductId { get; set; }
        public string? Notes { get; set; }
        public string? PickupAddress { get; set; }
        public string? DeliveryAddress { get; set; }
        public DateTime? PickupDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int? VehicleId { get; set; }
        public int? DriverId { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
