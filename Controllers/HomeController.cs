using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MetroFlow.Data;
using MetroFlow.Models;
using MetroFlow.Models.ViewModels;

namespace MetroFlow.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(AppDbContext db, UserManager<AppUser> um) { _db = db; _userManager = um; }

        public async Task<IActionResult> Index()
        {
            var stations = await _db.Stations.Where(s => s.IsActive).OrderBy(s => s.Line).ToListAsync();
            var hour = DateTime.Now.Hour;
            ViewBag.IsPeakHour = (hour >= 8 && hour <= 10) || (hour >= 17 && hour <= 19);
            ViewBag.CurrentTime = DateTime.Now.ToString("HH:mm");
            ViewBag.CriticalCount = stations.Count(s => s.OccupancyPercent >= 70);
            return View(stations);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var station = await _db.Stations
                .Include(s => s.OccupancyLogs.OrderByDescending(o => o.RecordedAt).Take(10))
                .Include(s => s.TrainSchedules.Where(t => t.IsActive)).ThenInclude(ts => ts.Train)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (station == null) return NotFound();

            var now = DateTime.Now.TimeOfDay;
            var upcoming = station.TrainSchedules.Where(ts => ts.ArrivalTime >= now).OrderBy(ts => ts.ArrivalTime).Take(5).ToList();

            return View(new StationDetailViewModel
            {
                Station = station,
                UpcomingTrains = upcoming,
                RecentLogs = station.OccupancyLogs.ToList(),
                Recommendation = GetRecommendation(station)
            });
        }

        private static string GetRecommendation(Station s)
        {
            var p = s.OccupancyPercent;
            var hour = DateTime.Now.Hour;
            var peak = (hour >= 8 && hour <= 10) || (hour >= 17 && hour <= 19);
            if (p < 40)  return "✅ Stansiya kifayət qədər boşdur. İndi rahat səyahət edə bilərsiniz.";
            if (p < 70)  return "⚠️ Orta sıxlıq var. Mümkünsə 15-20 dəqiqə gözləyin.";
            if (p < 90 && peak) return "🔴 Pik saatdır! Növbəti 2-3 qatar keçənə kimi gözləyin.";
            if (p >= 90) return "🚨 Kritik sıxlıq! Alternativ nəqliyyat istifadə edin və ya 30 dəq gözləyin.";
            return "ℹ️ Məlumatlar yenilənir...";
        }

        public IActionResult About()   => View();
        public IActionResult Contact() => View();
        public IActionResult Privacy() => View();
        public IActionResult Error()   => View();
    }
}
