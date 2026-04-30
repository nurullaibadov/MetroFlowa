using Microsoft.AspNetCore.Identity;

namespace MetroFlow.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? HomeStation { get; set; }
        public string? WorkStation { get; set; }
        public TimeSpan? PreferredMorningTime { get; set; }
        public TimeSpan? PreferredEveningTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
