using System.ComponentModel.DataAnnotations;

namespace back_end.BLL.DTOs
{
    public class VehicleDto
    {
        public int Id { get; set; }
        public string Model { get; set; } = string.Empty;
        public string PlateNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Capacity { get; set; }
        public int Year { get; set; }
        public bool IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateVehicleDto
    {
        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PlateNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = "Flatbed";

        [Range(0, double.MaxValue, ErrorMessage = "Capacity cannot be negative")]
        public decimal Capacity { get; set; }

        [Range(1900, 2100)]
        public int Year { get; set; }

        public bool IsAvailable { get; set; } = true;

        public string? ImageUrl { get; set; }
    }

    public class UpdateVehicleDto
    {
        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PlateNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Capacity { get; set; }

        [Range(1900, 2100)]
        public int Year { get; set; }

        public bool IsAvailable { get; set; }

        public string? ImageUrl { get; set; }
    }
}
