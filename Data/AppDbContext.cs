using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MetroFlow.Models;

namespace MetroFlow.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Station> Stations { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<TrainSchedule> TrainSchedules { get; set; }
        public DbSet<OccupancyLog> OccupancyLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Station>(e =>
            {
                e.Ignore(x => x.OccupancyPercent);
                e.Ignore(x => x.CrowdLevelAz);
                e.Ignore(x => x.CrowdColor);
                e.Ignore(x => x.BadgeClass);
            });

            builder.Entity<Train>(e =>
            {
                e.Ignore(x => x.OccupancyPercent);
                e.Ignore(x => x.StatusLabel);
                e.Ignore(x => x.BadgeClass);
            });

            builder.Entity<TrainSchedule>(e =>
            {
                e.Ignore(x => x.ArrivalTimeStr);
                e.Ignore(x => x.DepartureTimeStr);
                e.Ignore(x => x.IsPeakHour);
                e.HasOne(x => x.Train).WithMany(t => t.Schedules).HasForeignKey(x => x.TrainId);
                e.HasOne(x => x.Station).WithMany(s => s.TrainSchedules).HasForeignKey(x => x.StationId);
            });

            builder.Entity<OccupancyLog>(e =>
                e.HasOne(x => x.Station).WithMany(s => s.OccupancyLogs).HasForeignKey(x => x.StationId));

            builder.Entity<Notification>(e =>
                e.HasOne(x => x.User).WithMany(u => u.Notifications).HasForeignKey(x => x.UserId));

            // ── Seed Stations ──
            builder.Entity<Station>().HasData(
                new Station { Id=1,  Name="İçərişəhər",          Line="Qırmızı", Capacity=600,  CurrentOccupancy=120, Latitude=40.3660, Longitude=49.8371 },
                new Station { Id=2,  Name="Sahil",               Line="Qırmızı", Capacity=700,  CurrentOccupancy=200, Latitude=40.3712, Longitude=49.8389 },
                new Station { Id=3,  Name="28 May",              Line="Qırmızı", Capacity=1000, CurrentOccupancy=850, Latitude=40.3793, Longitude=49.8513 },
                new Station { Id=4,  Name="Gənclik",             Line="Qırmızı", Capacity=800,  CurrentOccupancy=400, Latitude=40.3879, Longitude=49.8527 },
                new Station { Id=5,  Name="Nəriman Nərimanov",   Line="Qırmızı", Capacity=700,  CurrentOccupancy=300, Latitude=40.4009, Longitude=49.8508 },
                new Station { Id=6,  Name="Elmlər Akademiyası",  Line="Qırmızı", Capacity=900,  CurrentOccupancy=820, Latitude=40.4096, Longitude=49.8677 },
                new Station { Id=7,  Name="İnşaatçılar",         Line="Qırmızı", Capacity=600,  CurrentOccupancy=180, Latitude=40.4197, Longitude=49.8709 },
                new Station { Id=8,  Name="Əhmədli",             Line="Qırmızı", Capacity=500,  CurrentOccupancy=100, Latitude=40.4098, Longitude=49.9050 },
                new Station { Id=9,  Name="Hövsan",              Line="Qırmızı", Capacity=400,  CurrentOccupancy=80,  Latitude=40.4038, Longitude=49.9399 },
                new Station { Id=10, Name="8 Noyabr",            Line="Yaşıl",   Capacity=600,  CurrentOccupancy=220, Latitude=40.4008, Longitude=49.8421 },
                new Station { Id=11, Name="Avtovağzal",          Line="Yaşıl",   Capacity=700,  CurrentOccupancy=310, Latitude=40.4108, Longitude=49.8269 },
                new Station { Id=12, Name="Memar Əcəmi",         Line="Yaşıl",   Capacity=650,  CurrentOccupancy=270, Latitude=40.4091, Longitude=49.8078 }
            );

            // ── Seed Trains ──
            builder.Entity<Train>().HasData(
                new Train { Id=1, TrainNumber="Q-001", Line="Qırmızı", Capacity=1200, CurrentPassengers=900 },
                new Train { Id=2, TrainNumber="Q-002", Line="Qırmızı", Capacity=1200, CurrentPassengers=400 },
                new Train { Id=3, TrainNumber="Q-003", Line="Qırmızı", Capacity=1200, CurrentPassengers=200 },
                new Train { Id=4, TrainNumber="Y-001", Line="Yaşıl",   Capacity=1000, CurrentPassengers=700 },
                new Train { Id=5, TrainNumber="Y-002", Line="Yaşıl",   Capacity=1000, CurrentPassengers=300 }
            );
        }
    }
}
