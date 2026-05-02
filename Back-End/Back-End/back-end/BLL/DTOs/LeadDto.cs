using System.ComponentModel.DataAnnotations;

namespace back_end.BLL.DTOs
{
    public class LeadDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Company { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string Equipment { get; set; } = string.Empty;
        public decimal? Weight { get; set; }
        public DateTime? PickupDate { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateLeadDto
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(30)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Company { get; set; }

        [Required]
        [MaxLength(150)]
        public string Origin { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Destination { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Equipment { get; set; } = string.Empty;

        public decimal? Weight { get; set; }

        public DateTime? PickupDate { get; set; }

        [MaxLength(2000)]
        public string Message { get; set; } = string.Empty;
    }

    public class UpdateLeadStatusDto
    {
        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = string.Empty;
    }
}
