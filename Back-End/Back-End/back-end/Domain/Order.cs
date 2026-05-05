namespace back_end.Domain
{
    public class Order
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

        public User User { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public Vehicle? Vehicle { get; set; }
        public Driver? Driver { get; set; }
    }
}
