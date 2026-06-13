namespace ClickExpress.Domain.Entities.User
{
    public class UserData
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        // Null for social-login (Google/Facebook) accounts that have no local password.
        public string? PasswordHash { get; set; }
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Authentication provider: "local" | "google" | "facebook".
        public string Provider { get; set; } = "local";
        // The user's unique id at the external provider (e.g. Google "sub"). Null for local accounts.
        public string? ProviderId { get; set; }

        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Company { get; set; }
        public string? Address { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
    }
}
