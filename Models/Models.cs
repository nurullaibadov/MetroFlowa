using System.ComponentModel.DataAnnotations;

namespace MetroFlow.Models
{
    public class Train
    {
        public int Id { get; set; }
        [Required, MaxLength(20)] public string TrainNumber { get; set; } = string.Empty;
        [Required, MaxLength(50)]  public string Line { get; set; } = string.Empty;
        public int Capacity { get; set; } = 1000;
        public int CurrentPassengers { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<TrainSchedule> Schedules { get; set; } = new List<TrainSchedule>();

        public double OccupancyPercent => Capacity > 0 ? Math.Round((double)CurrentPassengers / Capacity * 100, 1) : 0;
        public string StatusLabel => OccupancyPercent switch { < 40 => "Boş", < 70 => "Orta", < 90 => "Sıx", _ => "Kritik" };
        public string BadgeClass  => OccupancyPercent switch { < 40 => "success", < 70 => "warning", < 90 => "danger", _ => "dark" };
    }

    public class TrainSchedule
    {
        public int Id { get; set; }
        public int TrainId { get; set; }
        public Train Train { get; set; } = null!;
        public int StationId { get; set; }
        public Station Station { get; set; } = null!;
        public TimeSpan ArrivalTime { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public int PredictedOccupancyPercent { get; set; }
        public bool IsActive { get; set; } = true;

        public string ArrivalTimeStr    => ArrivalTime.ToString(@"hh\:mm");
        public string DepartureTimeStr  => DepartureTime.ToString(@"hh\:mm");
        public bool IsPeakHour =>
            (ArrivalTime >= TimeSpan.FromHours(8) && ArrivalTime <= TimeSpan.FromHours(10)) ||
            (ArrivalTime >= TimeSpan.FromHours(17) && ArrivalTime <= TimeSpan.FromHours(19));
    }

    public class OccupancyLog
    {
        public int Id { get; set; }
        public int StationId { get; set; }
        public Station Station { get; set; } = null!;
        public int? TrainId { get; set; }
        public Train? Train { get; set; }
        public int OccupancyPercent { get; set; }
        public int PassengerCount { get; set; }
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }
    }

    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "Info";
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
