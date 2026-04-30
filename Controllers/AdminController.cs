using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MetroFlow.Data;
using MetroFlow.Models;
using MetroFlow.Models.ViewModels;

namespace MetroFlow.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(AppDbContext db, UserManager<AppUser> um, RoleManager<IdentityRole> rm)
        { _db = db; _userManager = um; _roleManager = rm; }

        // ── Dashboard ──────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                TotalUsers     = await _db.Users.CountAsync(),
                TotalStations  = await _db.Stations.CountAsync(),
                TotalTrains    = await _db.Trains.CountAsync(),
                ActiveTrains   = await _db.Trains.CountAsync(t => t.IsActive),
                CriticalStations = await _db.Stations.Where(s => s.CurrentOccupancy >= s.Capacity * 0.7).ToListAsync(),
                RecentLogs     = await _db.OccupancyLogs.Include(o => o.Station).OrderByDescending(o => o.RecordedAt).Take(10).ToListAsync(),
                RecentUsers    = await _db.Users.OrderByDescending(u => u.CreatedAt).Take(5).ToListAsync()
            };
            return View(model);
        }

        // ── Users ──────────────────────────────────────────────
        public async Task<IActionResult> Users(string? q, int page = 1)
        {
            var query = _db.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(u => u.FullName.Contains(q) || (u.Email != null && u.Email.Contains(q)));

            var total = await query.CountAsync();
            var users = await query.OrderByDescending(u => u.CreatedAt).Skip((page - 1) * 20).Take(20).ToListAsync();

            return View(new AdminUserListViewModel
            {
                Users = users, TotalCount = total, SearchQuery = q, Page = page
            });
        }

        public async Task<IActionResult> UserDetail(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = roles;
            return View(user);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);
            TempData["Success"] = user.IsActive ? "İstifadəçi aktiv edildi." : "İstifadəçi deaktiv edildi.";
            return RedirectToAction("Users");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            await _userManager.DeleteAsync(user);
            TempData["Success"] = "İstifadəçi silindi.";
            return RedirectToAction("Users");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
                await _userManager.AddToRoleAsync(user, "Admin");
            TempData["Success"] = "İstifadəçiyə Admin rolu verildi.";
            return RedirectToAction("UserDetail", new { id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            await _userManager.RemoveFromRoleAsync(user, "Admin");
            TempData["Success"] = "Admin rolu götürüldü.";
            return RedirectToAction("UserDetail", new { id });
        }

        // ── Stations ───────────────────────────────────────────
        public async Task<IActionResult> Stations()
        {
            var stations = await _db.Stations.OrderBy(s => s.Line).ThenBy(s => s.Name).ToListAsync();
            return View(stations);
        }

        [HttpGet] public IActionResult CreateStation() => View(new StationCreateEditViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStation(StationCreateEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            _db.Stations.Add(new Station
            {
                Name = model.Name, Line = model.Line, Capacity = model.Capacity,
                Latitude = model.Latitude, Longitude = model.Longitude, IsActive = model.IsActive
            });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Stansiya əlavə edildi!";
            return RedirectToAction("Stations");
        }

        [HttpGet] public async Task<IActionResult> EditStation(int id)
        {
            var s = await _db.Stations.FindAsync(id);
            if (s == null) return NotFound();
            return View(new StationCreateEditViewModel
            {
                Id = s.Id, Name = s.Name, Line = s.Line, Capacity = s.Capacity,
                Latitude = s.Latitude, Longitude = s.Longitude, IsActive = s.IsActive
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStation(StationCreateEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var s = await _db.Stations.FindAsync(model.Id);
            if (s == null) return NotFound();
            s.Name = model.Name; s.Line = model.Line; s.Capacity = model.Capacity;
            s.Latitude = model.Latitude; s.Longitude = model.Longitude; s.IsActive = model.IsActive;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Stansiya yeniləndi!";
            return RedirectToAction("Stations");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStation(int id)
        {
            var s = await _db.Stations.FindAsync(id);
            if (s == null) return NotFound();
            _db.Stations.Remove(s);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Stansiya silindi.";
            return RedirectToAction("Stations");
        }

        // ── Trains ─────────────────────────────────────────────
        public async Task<IActionResult> Trains()
        {
            var trains = await _db.Trains.OrderBy(t => t.Line).ThenBy(t => t.TrainNumber).ToListAsync();
            return View(trains);
        }

        [HttpGet] public IActionResult CreateTrain() => View(new TrainCreateEditViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTrain(TrainCreateEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            _db.Trains.Add(new Train { TrainNumber = model.TrainNumber, Line = model.Line, Capacity = model.Capacity, IsActive = model.IsActive });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Qatar əlavə edildi!";
            return RedirectToAction("Trains");
        }

        [HttpGet] public async Task<IActionResult> EditTrain(int id)
        {
            var t = await _db.Trains.FindAsync(id);
            if (t == null) return NotFound();
            return View(new TrainCreateEditViewModel { Id = t.Id, TrainNumber = t.TrainNumber, Line = t.Line, Capacity = t.Capacity, IsActive = t.IsActive });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrain(TrainCreateEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var t = await _db.Trains.FindAsync(model.Id);
            if (t == null) return NotFound();
            t.TrainNumber = model.TrainNumber; t.Line = model.Line; t.Capacity = model.Capacity; t.IsActive = model.IsActive;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Qatar yeniləndi!";
            return RedirectToAction("Trains");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTrain(int id)
        {
            var t = await _db.Trains.FindAsync(id);
            if (t == null) return NotFound();
            _db.Trains.Remove(t);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Qatar silindi.";
            return RedirectToAction("Trains");
        }

        // ── Occupancy Update ───────────────────────────────────
        [HttpGet] public async Task<IActionResult> UpdateOccupancy()
        {
            ViewBag.Stations = await _db.Stations.Where(s => s.IsActive).ToListAsync();
            return View(new UpdateOccupancyViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOccupancy(UpdateOccupancyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Stations = await _db.Stations.Where(s => s.IsActive).ToListAsync();
                return View(model);
            }
            var station = await _db.Stations.FindAsync(model.StationId);
            if (station == null) return NotFound();

            var passengers = model.PassengerCount > 0 ? model.PassengerCount : (int)(station.Capacity * model.OccupancyPercent / 100.0);
            station.CurrentOccupancy = passengers;

            _db.OccupancyLogs.Add(new OccupancyLog
            {
                StationId = model.StationId, OccupancyPercent = model.OccupancyPercent,
                PassengerCount = passengers, Notes = model.Notes, RecordedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
            TempData["Success"] = $"{station.Name} stansiyasının dolululuğu yeniləndi: {model.OccupancyPercent}%";
            return RedirectToAction("Stations");
        }

        // ── Logs ───────────────────────────────────────────────
        public async Task<IActionResult> Logs(int? stationId)
        {
            ViewBag.Stations = await _db.Stations.ToListAsync();
            var query = _db.OccupancyLogs.Include(o => o.Station).AsQueryable();
            if (stationId.HasValue) query = query.Where(o => o.StationId == stationId);
            var logs = await query.OrderByDescending(o => o.RecordedAt).Take(100).ToListAsync();
            ViewBag.SelectedStation = stationId;
            return View(logs);
        }

        // ── Notifications ──────────────────────────────────────
        public async Task<IActionResult> Notifications()
        {
            var notifs = await _db.Notifications.Include(n => n.User).OrderByDescending(n => n.CreatedAt).Take(50).ToListAsync();
            return View(notifs);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SendNotification(string userId, string title, string message, string type = "Info")
        {
            _db.Notifications.Add(new Notification { UserId = userId, Title = title, Message = message, Type = type });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Bildiriş göndərildi!";
            return RedirectToAction("Notifications");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SendBroadcast(string title, string message, string type = "Warning")
        {
            var users = await _db.Users.Select(u => u.Id).ToListAsync();
            foreach (var uid in users)
                _db.Notifications.Add(new Notification { UserId = uid, Title = title, Message = message, Type = type });
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Bütün {users.Count} istifadəçiyə bildiriş göndərildi!";
            return RedirectToAction("Notifications");
        }
    }
}
