using System.ComponentModel.DataAnnotations;

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

    public class CreateDriverDTO
    {
        [Required][MaxLength(100)] public string FullName { get; set; } = string.Empty;
        [Required][MaxLength(20)] public string Phone { get; set; } = string.Empty;
        [Required][MaxLength(30)] public string CdlNumber { get; set; } = string.Empty;
        [MaxLength(50)] public string Status { get; set; } = "Active";
        public int? VehicleId { get; set; }
    }

    public class UpdateDriverDTO
    {
        [Required][MaxLength(100)] public string FullName { get; set; } = string.Empty;
        [Required][MaxLength(20)] public string Phone { get; set; } = string.Empty;
        [Required][MaxLength(30)] public string CdlNumber { get; set; } = string.Empty;
        [Required][MaxLength(50)] public string Status { get; set; } = string.Empty;
        public int? VehicleId { get; set; }
    }

    public class PatchDriverStatusDTO
    {
        [Required][MaxLength(50)] public string Status { get; set; } = string.Empty;
    }
}
