using System.ComponentModel.DataAnnotations;

namespace MetroFlow.Models
{
    public class Station
    {
        public int Id { get; set; }
        [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
        [Required, MaxLength(50)]  public string Line { get; set; } = string.Empty;
        public int Capacity { get; set; } = 500;
        public int CurrentOccupancy { get; set; } = 0;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OccupancyLog> OccupancyLogs { get; set; } = new List<OccupancyLog>();
        public ICollection<TrainSchedule> TrainSchedules { get; set; } = new List<TrainSchedule>();

        public double OccupancyPercent => Capacity > 0 ? Math.Round((double)CurrentOccupancy / Capacity * 100, 1) : 0;
        public string CrowdLevelAz => OccupancyPercent switch { < 40 => "Az", < 70 => "Orta", < 90 => "Sıx", _ => "Kritik" };
        public string CrowdColor   => OccupancyPercent switch { < 40 => "#22c55e", < 70 => "#f59e0b", < 90 => "#ef4444", _ => "#7f1d1d" };
        public string BadgeClass   => OccupancyPercent switch { < 40 => "success", < 70 => "warning", < 90 => "danger", _ => "dark" };
    }
}
