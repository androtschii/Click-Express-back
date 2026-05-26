using ClickExpress.Domain.Entities.Vehicle;

namespace ClickExpress.Domain.Entities.Driver
{
    public class DriverData
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string CdlNumber { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public int? VehicleId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public VehicleData? Vehicle { get; set; }
    }
}
