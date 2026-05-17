using System.ComponentModel.DataAnnotations;

namespace ClickExpress.Domain.Models.Driver
{
    public class UpdateDriverDTO
    {
        [Required][MaxLength(100)] public string FullName { get; set; } = string.Empty;
        [Required][MaxLength(20)] public string Phone { get; set; } = string.Empty;
        [Required][MaxLength(30)] public string CdlNumber { get; set; } = string.Empty;
        [Required][MaxLength(50)] public string Status { get; set; } = string.Empty;
        public int? VehicleId { get; set; }
    }
}
