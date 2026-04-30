using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MetroFlow.Data;
using MetroFlow.Models.ViewModels;

namespace MetroFlow.Controllers
{
    public class StationController : Controller
    {
        private readonly AppDbContext _db;
        public StationController(AppDbContext db) { _db = db; }

        public async Task<IActionResult> Index(string? line)
        {
            var query = _db.Stations.Where(s => s.IsActive);
            if (!string.IsNullOrEmpty(line)) query = query.Where(s => s.Line == line);
            var stations = await query.OrderBy(s => s.Line).ThenBy(s => s.Name).ToListAsync();
            ViewBag.Lines = await _db.Stations.Select(s => s.Line).Distinct().ToListAsync();
            ViewBag.SelectedLine = line;
            return View(stations);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var station = await _db.Stations
                .Include(s => s.OccupancyLogs.OrderByDescending(o => o.RecordedAt).Take(10))
                .Include(s => s.TrainSchedules.Where(t => t.IsActive)).ThenInclude(ts => ts.Train)
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);

            if (station == null) return NotFound();

            var now = DateTime.Now.TimeOfDay;
            var upcoming = station.TrainSchedules.Where(ts => ts.ArrivalTime >= now)
                .OrderBy(ts => ts.ArrivalTime).Take(5).ToList();

            var p = station.OccupancyPercent;
            var hour = DateTime.Now.Hour;
            var peak = (hour >= 8 && hour <= 10) || (hour >= 17 && hour <= 19);

            string rec = p < 40 ? "✅ Stansiya boşdur, rahat gedə bilərsiniz." :
                         p < 70 ? "⚠️ Orta sıxlıq, mümkünsə 15 dəq gözləyin." :
                         p < 90 && peak ? "🔴 Pik saat! 2-3 qatar keçənə kimi gözləyin." :
                         p >= 90 ? "🚨 Kritik! Alternativ nəqliyyat istifadə edin." : "ℹ️ Yenilənir...";

            return View(new StationDetailViewModel
            {
                Station = station, UpcomingTrains = upcoming,
                RecentLogs = station.OccupancyLogs.ToList(), Recommendation = rec
            });
        }

        public async Task<IActionResult> Map()
        {
            var stations = await _db.Stations.Where(s => s.IsActive).ToListAsync();
            return View(stations);
        }
    }
}
