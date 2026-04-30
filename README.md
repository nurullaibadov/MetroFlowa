# MetroFlow 🚇
**Bakı Metrosu Real-Time Sıxlıq İdarəetmə Sistemi**

ASP.NET Core 8 MVC + SQL Server + Entity Framework Core + Bootstrap 5

---

## ⚡ Sürətli Başlangıc

### 1. Tələblər
- .NET 8 SDK
- SQL Server (LocalDB/Express/Full)
- Visual Studio 2022 və ya VS Code

### 2. appsettings.json — Connection String
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=MetroFlowDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```
LocalDB istifadə etsəniz:
```
Server=(localdb)\mssqllocaldb;Database=MetroFlowDb;Trusted_Connection=True;
```

### 3. Migration & Database
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```
> Proqram ilk işə düşəndə migration avtomatik tətbiq olunur.

### 4. Layihəni İşə Sal
```bash
dotnet run
```
Brauzerinizi açın: **https://localhost:5001**

---

## 🔐 Default Admin Hesabı
| Email | Şifrə |
|-------|-------|
| admin@metroflow.az | Admin@123 |

---

## 📁 Layihə Strukturu
```
MetroFlow/
├── Controllers/
│   ├── AccountController.cs   ← Login, Register, ForgotPassword, Profile
│   ├── AdminController.cs     ← Dashboard, Users, Stations, Trains, Logs
│   ├── HomeController.cs      ← Ana səhifə
│   └── StationController.cs   ← Stansiya siyahısı, detail, xəritə
├── Models/
│   ├── AppUser.cs
│   ├── Models.cs              ← Train, TrainSchedule, OccupancyLog, Notification
│   ├── Station.cs
│   └── ViewModels/
├── Views/
│   ├── Account/               ← Login, Register, ForgotPassword, ResetPassword, Profile
│   ├── Admin/                 ← Dashboard, Users, Stations, Trains, Logs, Notifications
│   ├── Home/                  ← Index, About
│   ├── Station/               ← Index, Detail, Map
│   └── Shared/_Layout.cshtml
├── Data/AppDbContext.cs
├── Program.cs
└── appsettings.json
```

---

## 🎯 Əsas Xüsusiyyətlər
- ✅ Login / Register / Forgot Password / Reset Password
- ✅ Admin Panel — İstifadəçi, Stansiya, Qatar idarəetməsi
- ✅ Real-time dolululuq göstəricisi (progress bar)
- ✅ Sıxlıq tövsiyəsi (Az / Orta / Sıx / Kritik)
- ✅ Leaflet.js xəritəsi — stansiya markerləri
- ✅ Seed data — 12 Bakı metro stansiyası + 5 qatar
- ✅ Role-based authorization (Admin / User)
- ✅ Broadcast bildiriş sistemi

---

## 📦 NuGet Packages
```
Microsoft.AspNetCore.Identity.EntityFrameworkCore 8.0.0
Microsoft.EntityFrameworkCore.SqlServer 8.0.0
Microsoft.EntityFrameworkCore.Tools 8.0.0
Microsoft.AspNetCore.Identity.UI 8.0.0
Microsoft.AspNetCore.SignalR 1.1.0
```

---

## 🛠 Migration Komandaları
```bash
# İlk migration
dotnet ef migrations add InitialCreate

# DB yenilə
dotnet ef database update

# Migration sil (lazım olsa)
dotnet ef migrations remove
```
