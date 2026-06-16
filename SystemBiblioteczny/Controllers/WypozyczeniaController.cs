using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SystemBiblioteczny.Data;
using SystemBiblioteczny.Models;

namespace SystemBiblioteczny.Controllers
{
    public class WypozyczeniaController : Controller
    {
        private readonly BibliotekaDbContext _context;

        public WypozyczeniaController(BibliotekaDbContext context)
        {
            _context = context;
        }

        // LISTA WSZYSTKICH (Tylko dla Bibliotekarza)
        [Authorize(Policy = "TylkoBibliotekarze")]
        public async Task<IActionResult> Index()
        {
            var wypozyczenia = await _context.Wypozyczenia
                .Include(w => w.Uzytkownik)
                .Include(w => w.Egzemplarz)
                    .ThenInclude(e => e.Ksiazka)
                .ToListAsync();

            return View(wypozyczenia);
        }

        // =====================
        // WYPOŻYCZ (Tylko dla Czytelnika)
        // =====================
        [Authorize(Policy = "TylkoCzytelnicy")]
        public async Task<IActionResult> Wypozycz(int ksiazkaId)
        {
            var uzytkownikIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(uzytkownikIdString) || !int.TryParse(uzytkownikIdString, out int uzytkownikId))
            {
                TempData["Blad"] = "Musisz być zalogowany jako czytelnik.";
                return RedirectToAction("Login", "Account");
            }

            var egzemplarz = await _context.Egzemplarze
                .FirstOrDefaultAsync(e =>
                    e.KsiazkaId == ksiazkaId &&
                    e.Status == StatusEgzemplarza.Dostepny);

            if (egzemplarz == null)
            {
                TempData["Blad"] = "Brak dostępnych egzemplarzy.";
                return RedirectToAction("Index", "Ksiazki");
            }

            bool maNieoplaconeKary = await _context.Kary
                .AnyAsync(k =>
                    k.UzytkownikId == uzytkownikId &&
                    k.DataZaplaty == null);

            if (maNieoplaconeKary)
            {
                TempData["Blad"] = "Czytelnik posiada nieopłacone kary.";
                return RedirectToAction("Index", "Ksiazki");
            }

            var wypozyczenie = new Wypozyczenie
            {
                DataWypozyczenia = DateOnly.FromDateTime(DateTime.Now),
                PlanowanyZwrot = DateOnly.FromDateTime(DateTime.Today.AddDays(30)),
                Status = StatusWypozyczenia.Aktywne,
                EgzemplarzId = egzemplarz.EgzemplarzId,
                UzytkownikId = uzytkownikId
            };

            egzemplarz.Status = StatusEgzemplarza.Wypozyczony;
            _context.Wypozyczenia.Add(wypozyczenie);
            await _context.SaveChangesAsync();

            TempData["Sukces"] = "Książka została wypożyczona!";

            // Po wypożyczeniu przekierowujemy czytelnika od razu do jego panelu
            return RedirectToAction(nameof(MojeWypozyczenia));
        }

        // =====================
        // ZWROT (Tylko dla Bibliotekarza)
        // =====================
        [Authorize(Policy = "TylkoBibliotekarze")]
        public async Task<IActionResult> Zwroc(int id)
        {
            var wypozyczenie = await _context.Wypozyczenia
                .Include(w => w.Egzemplarz)
                .FirstOrDefaultAsync(w => w.WypozyczanieId == id);

            if (wypozyczenie == null)
                return NotFound();

            var dzis = DateOnly.FromDateTime(DateTime.Today);

            wypozyczenie.DataZwrotu = dzis;
            wypozyczenie.Status = StatusWypozyczenia.Zwrocone;
            wypozyczenie.Egzemplarz.Status = StatusEgzemplarza.Dostepny;

            if (dzis > wypozyczenie.PlanowanyZwrot)
            {
                int dniSpoznienia = dzis.DayNumber - wypozyczenie.PlanowanyZwrot.DayNumber;

                var kara = new Kara
                {
                    UzytkownikId = wypozyczenie.UzytkownikId,
                    BibliotekarzeId = wypozyczenie.BibliotekarzeId,
                    WypozyczanieId = wypozyczenie.WypozyczanieId,
                    Kwota = dniSpoznienia * 2m,
                    DataNalozenia = dzis,
                    Powod = $"Przekroczenie terminu o {dniSpoznienia} dni"
                };

                _context.Kary.Add(kara);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // =====================
        // PRZEDŁUŻ (Zalogowani)
        // =====================
        [Authorize(Policy = "Zalogowani")]
        public async Task<IActionResult> Przedluz(int id)
        {
            var wypozyczenie = await _context.Wypozyczenia
                .FirstOrDefaultAsync(w => w.WypozyczanieId == id);

            if (wypozyczenie == null)
                return NotFound();

            if (wypozyczenie.LiczbaPrzedluzen >= 2)
            {
                TempData["Blad"] = "Osiągnięto maksymalną liczbę przedłużeń.";
                return User.IsInRole("Bibliotekarz")
                    ? RedirectToAction(nameof(Index))
                    : RedirectToAction(nameof(MojeWypozyczenia));
            }

            wypozyczenie.PlanowanyZwrot = wypozyczenie.PlanowanyZwrot.AddDays(14);
            wypozyczenie.LiczbaPrzedluzen++;

            await _context.SaveChangesAsync();
            TempData["Sukces"] = "Termin zwrotu został przedłużony o 14 dni.";

            // POPRAWKA: Przekierowanie w zależności od roli
            if (User.IsInRole("Bibliotekarz"))
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(MojeWypozyczenia));
        }

        // =====================
        // NOWOŚĆ: PANEL CZYTELNIKA
        // =====================
        [Authorize(Policy = "TylkoCzytelnicy")]
        public async Task<IActionResult> MojeWypozyczenia()
        {
            var uzytkownikIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(uzytkownikIdString) || !int.TryParse(uzytkownikIdString, out int uzytkownikId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Pobieramy wypożyczenia zalogowanego użytkownika wraz z danymi o książce
            var mojeWypozyczenia = await _context.Wypozyczenia
                .Include(w => w.Egzemplarz)
                    .ThenInclude(e => e.Ksiazka)
                .Where(w => w.UzytkownikId == uzytkownikId)
                .OrderByDescending(w => w.DataWypozyczenia)
                .ToListAsync();

            return View(mojeWypozyczenia);
        }
    }
}