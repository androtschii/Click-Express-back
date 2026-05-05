using System.ComponentModel.DataAnnotations;

namespace back_end.BLL.DTOs
{
    public class DriverDto
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

    public class CreateDriverDto
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string CdlNumber { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Status { get; set; } = "Active";

        public int? VehicleId { get; set; }
    }

    public class UpdateDriverDto
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string CdlNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        public int? VehicleId { get; set; }
    }

    public class PatchDriverStatusDto
    {
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;
    }
}
