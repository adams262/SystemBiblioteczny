using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using SystemBiblioteczny.Data;
using SystemBiblioteczny.Models;
using SystemBiblioteczny.Models.ViewModels;

namespace SystemBiblioteczny.Controllers
{
    public class AccountController : Controller
    {
        private readonly BibliotekaDbContext _context;

        public AccountController(BibliotekaDbContext context)
        {
            _context = context;
        }

        // Login
        

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToHome();

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            // Sprawdź bibliotekarza
            var bibliotekarz = await _context.Bibliotekarze
                .FirstOrDefaultAsync(b => b.Email == model.Email);

            if (bibliotekarz != null && VerifyPassword(model.Haslo, bibliotekarz.PasswordHash))
            {
                await SignInUser(bibliotekarz.BibliotekarzeId.ToString(), bibliotekarz.Email,
                    bibliotekarz.Imie + " " + bibliotekarz.Nazwisko, "Bibliotekarz");

                return RedirectToLocal(returnUrl) ?? RedirectToAction("Index", "Home");
            }

            // Sprawdź czytelnika 
            var uzytkownik = await _context.Uzytkownicy
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (uzytkownik != null && VerifyPassword(model.Haslo, uzytkownik.PasswordHash))
            {
                await SignInUser(uzytkownik.UzytkownikId.ToString(), uzytkownik.Email,
                    uzytkownik.Imie + " " + uzytkownik.Nazwisko, "Czytelnik");

                return RedirectToLocal(returnUrl) ?? RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Nieprawidłowy e-mail lub hasło.");
            return View(model);
        }

        // Rejestracja czytelnika

        [HttpGet]
        public IActionResult RejestracjaCzytelnika()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToHome();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejestracjaCzytelnika(RejestracjaCzytelnikaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool emailZajety = await _context.Uzytkownicy.AnyAsync(u => u.Email == model.Email)
                            || await _context.Bibliotekarze.AnyAsync(b => b.Email == model.Email);

            if (emailZajety)
            {
                ModelState.AddModelError("Email", "Ten adres e-mail jest już zajęty.");
                return View(model);
            }

            var uzytkownik = new Uzytkownik
            {
                Imie = model.Imie,
                Nazwisko = model.Nazwisko,
                Email = model.Email,
                PasswordHash = HashPassword(model.Haslo),
                DataRejestracji = DateTime.Now
            };

            _context.Uzytkownicy.Add(uzytkownik);
            await _context.SaveChangesAsync();

            TempData["Sukces"] = "Konto zostało utworzone. Możesz się teraz zalogować.";
            return RedirectToAction(nameof(Login));
        }

        // Rejestracja bibliotekarz

        [HttpGet]
        public IActionResult RejestracjaBibliotekarza()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToHome();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejestracjaBibliotekarza(RejestracjaBibliotekarzeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool emailZajety = await _context.Uzytkownicy.AnyAsync(u => u.Email == model.Email)
                            || await _context.Bibliotekarze.AnyAsync(b => b.Email == model.Email);

            if (emailZajety)
            {
                ModelState.AddModelError("Email", "Ten adres e-mail jest już zajęty.");
                return View(model);
            }

            
            if (model.KodDostepu != "BIBLIOTEKA2026")
            {
                ModelState.AddModelError("KodDostepu", "Nieprawidłowy kod dostępu dla bibliotekarzy.");
                return View(model);
            }

            var bibliotekarz = new Bibliotekarz
            {
                Imie = model.Imie,
                Nazwisko = model.Nazwisko,
                Email = model.Email,
                PasswordHash = HashPassword(model.Haslo),
                DataZatrudnienia = DateTime.Now
            };

            _context.Bibliotekarze.Add(bibliotekarz);
            await _context.SaveChangesAsync();

            TempData["Sukces"] = "Konto bibliotekarza zostało utworzone. Możesz się teraz zalogować.";
            return RedirectToAction(nameof(Login));
        }

        // Wylogowywanie

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        

        public IActionResult AccessDenied()
        {
            return View();
        }

        

        private async Task SignInUser(string id, string email, string nazwaUzytkownika, string rola)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, nazwaUzytkownika),
                new Claim(ClaimTypes.Role, rola)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) }
            );
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "BibliotekaSalt2024"));
            return Convert.ToBase64String(bytes);
        }

        private static bool VerifyPassword(string password, string hash)
            => HashPassword(password) == hash;

        private IActionResult RedirectToHome()
            => User.IsInRole("Bibliotekarz")
                ? RedirectToAction("Index", "Home")
                : RedirectToAction("Index", "Home");

        private IActionResult? RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return null;
        }
    }
}
