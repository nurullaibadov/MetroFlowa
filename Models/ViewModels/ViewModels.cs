using System.ComponentModel.DataAnnotations;

namespace MetroFlow.Models.ViewModels
{
    // ─── ACCOUNT ─────────────────────────────────────────────
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email tələb olunur")]
        [EmailAddress(ErrorMessage = "Düzgün email daxil edin")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifrə tələb olunur")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Məni xatırla")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad Soyad tələb olunur")]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email tələb olunur")]
        [EmailAddress(ErrorMessage = "Düzgün email daxil edin")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifrə tələb olunur")]
        [MinLength(6, ErrorMessage = "Şifrə minimum 6 simvol olmalıdır")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifrə")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifrəni təkrar daxil edin")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifrələr uyğun gəlmir")]
        [Display(Name = "Şifrəni Təkrarla")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "Ev Stansiyası")]
        public string? HomeStation { get; set; }

        [Display(Name = "İş Stansiyası")]
        public string? WorkStation { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email tələb olunur")]
        [EmailAddress(ErrorMessage = "Düzgün email daxil edin")]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordViewModel
    {
        [Required] public string Token { get; set; } = string.Empty;
        [Required] [EmailAddress] public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yeni şifrə tələb olunur")]
        [MinLength(6, ErrorMessage = "Şifrə minimum 6 simvol olmalıdır")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifrə")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Şifrələr uyğun gəlmir")]
        [Display(Name = "Şifrəni Təkrarla")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class ProfileViewModel
    {
        [Required] [Display(Name = "Ad Soyad")] public string FullName { get; set; } = string.Empty;
        [Display(Name = "Ev Stansiyası")]  public string? HomeStation { get; set; }
        [Display(Name = "İş Stansiyası")]  public string? WorkStation { get; set; }
        [Display(Name = "Səhər Vaxtı")]    public string? PreferredMorningTime { get; set; }
        [Display(Name = "Axşam Vaxtı")]    public string? PreferredEveningTime { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Cari şifrə tələb olunur")]
        [DataType(DataType.Password)]
        [Display(Name = "Cari Şifrə")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yeni şifrə tələb olunur")]
        [MinLength(6)]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifrə")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Şifrələr uyğun gəlmir")]
        [Display(Name = "Şifrəni Təkrarla")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    // ─── ADMIN ───────────────────────────────────────────────
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalStations { get; set; }
        public int TotalTrains { get; set; }
        public int ActiveTrains { get; set; }
        public List<Station> CriticalStations { get; set; } = new();
        public List<OccupancyLog> RecentLogs { get; set; } = new();
        public List<AppUser> RecentUsers { get; set; } = new();
    }

    public class AdminUserListViewModel
    {
        public List<AppUser> Users { get; set; } = new();
        public int TotalCount { get; set; }
        public string? SearchQuery { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    // ─── STATION ─────────────────────────────────────────────
    public class StationCreateEditViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Stansiya adı tələb olunur")]
        [Display(Name = "Stansiya Adı")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xətt tələb olunur")]
        [Display(Name = "Metro Xətti")]
        public string Line { get; set; } = string.Empty;

        [Range(100, 5000)]
        [Display(Name = "Tutum (nəfər)")]
        public int Capacity { get; set; } = 500;

        [Display(Name = "Enlik")]  public double? Latitude { get; set; }
        [Display(Name = "Uzunluq")] public double? Longitude { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class StationDetailViewModel
    {
        public Station Station { get; set; } = null!;
        public List<TrainSchedule> UpcomingTrains { get; set; } = new();
        public List<OccupancyLog> RecentLogs { get; set; } = new();
        public string Recommendation { get; set; } = string.Empty;
    }

    // ─── TRAIN ───────────────────────────────────────────────
    public class TrainCreateEditViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Qatar nömrəsi tələb olunur")]
        [Display(Name = "Qatar Nömrəsi")]
        public string TrainNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xətt tələb olunur")]
        [Display(Name = "Xətt")]
        public string Line { get; set; } = string.Empty;

        [Range(100, 5000)]
        [Display(Name = "Tutum")]
        public int Capacity { get; set; } = 1000;
        public bool IsActive { get; set; } = true;
    }

    public class UpdateOccupancyViewModel
    {
        [Required] public int StationId { get; set; }
        [Required] [Range(0, 100)] public int OccupancyPercent { get; set; }
        public int PassengerCount { get; set; }
        public string? Notes { get; set; }
    }
}
