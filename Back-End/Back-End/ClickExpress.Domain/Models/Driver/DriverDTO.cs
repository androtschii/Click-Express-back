namespace ClickExpress.Domain.Models.Driver
{
    public class DriverDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string CdlNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int? VehicleId { get; set; }
        public string? VehicleModel { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
