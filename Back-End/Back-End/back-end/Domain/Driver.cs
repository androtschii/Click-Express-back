namespace back_end.Domain
{
    public class Driver
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string CdlNumber { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public int? VehicleId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Vehicle? Vehicle { get; set; }
    }
}
