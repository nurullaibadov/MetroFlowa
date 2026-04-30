using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MetroFlow.Models;
using MetroFlow.Models.ViewModels;

namespace MetroFlow.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<AppUser> um, SignInManager<AppUser> sm, ILogger<AccountController> logger)
        { _userManager = um; _signInManager = sm; _logger = logger; }

        // ── Login ──────────────────────────────────────────────
        [HttpGet] public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError("", user == null ? "Email və ya şifrə yanlışdır." : "Hesabınız deaktiv edilmişdir.");
                return View(model);
            }
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", result.IsLockedOut ? "Hesab bloklanmışdır. Sonra cəhd edin." : "Email və ya şifrə yanlışdır.");
            return View(model);
        }

        // ── Register ───────────────────────────────────────────
        [HttpGet] public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                ModelState.AddModelError("Email", "Bu email artıq istifadə olunur.");
                return View(model);
            }
            var user = new AppUser
            {
                FullName = model.FullName, UserName = model.Email, Email = model.Email,
                HomeStation = model.HomeStation, WorkStation = model.WorkStation,
                IsActive = true, CreatedAt = DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, false);
                TempData["Success"] = "Qeydiyyat uğurla tamamlandı!";
                return RedirectToAction("Index", "Home");
            }
            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            return View(model);
        }

        // ── Forgot Password ────────────────────────────────────
        [HttpGet] public IActionResult ForgotPassword() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var link  = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme);
                _logger.LogInformation("Reset link: {Link}", link);
                TempData["ResetLink"] = link;
            }
            return RedirectToAction("ForgotPasswordConfirmation");
        }

        [HttpGet] public IActionResult ForgotPasswordConfirmation() => View();

        // ── Reset Password ─────────────────────────────────────
        [HttpGet] public IActionResult ResetPassword(string? token, string? email)
        {
            if (token == null || email == null) return BadRequest();
            return View(new ResetPasswordViewModel { Token = token, Email = email });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) { TempData["Success"] = "Şifrə dəyişdirildi!"; return RedirectToAction("Login"); }
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded) { TempData["Success"] = "Şifrəniz uğurla dəyişdirildi!"; return RedirectToAction("Login"); }
            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            return View(model);
        }

        // ── Profile ────────────────────────────────────────────
        [Authorize, HttpGet] public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");
            return View(new ProfileViewModel
            {
                FullName = user.FullName, HomeStation = user.HomeStation, WorkStation = user.WorkStation,
                PreferredMorningTime = user.PreferredMorningTime?.ToString(@"hh\:mm"),
                PreferredEveningTime = user.PreferredEveningTime?.ToString(@"hh\:mm")
            });
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");
            user.FullName = model.FullName; user.HomeStation = model.HomeStation; user.WorkStation = model.WorkStation;
            if (TimeSpan.TryParse(model.PreferredMorningTime, out var mt)) user.PreferredMorningTime = mt;
            if (TimeSpan.TryParse(model.PreferredEveningTime, out var et)) user.PreferredEveningTime = et;
            await _userManager.UpdateAsync(user);
            TempData["Success"] = "Profiliniz yeniləndi!";
            return RedirectToAction("Profile");
        }

        // ── Change Password ────────────────────────────────────
        [Authorize, HttpGet] public IActionResult ChangePassword() => View();

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded) { await _signInManager.RefreshSignInAsync(user); TempData["Success"] = "Şifrəniz dəyişdirildi!"; return RedirectToAction("Profile"); }
            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            return View(model);
        }

        // ── Logout ─────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        { await _signInManager.SignOutAsync(); return RedirectToAction("Index", "Home"); }

        public IActionResult AccessDenied() => View();
    }
}
