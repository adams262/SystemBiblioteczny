using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemBiblioteczny.Data;

namespace SystemBiblioteczny.Controllers
{
    [Authorize(Policy = "TylkoBibliotekarze")] // Tylko personel ma tu wstęp
    public class UzytkownicyController : Controller
    {
        private readonly BibliotekaDbContext _context;

        public UzytkownicyController(BibliotekaDbContext context)
        {
            _context = context;
        }

        // ─── LISTA CZYTELNIKÓW ─────────────────────────────────
        public async Task<IActionResult> Index()
        {
            // Pobieramy użytkowników i od razu liczymy ich aktywne wypożyczenia
            var czytelnicy = await _context.Uzytkownicy
                .Include(u => u.Wypozyczenia)
                .ToListAsync();

            return View(czytelnicy);
        }

        // ─── SZCZEGÓŁY CZYTELNIKA I JEGO KSIĄŻKI ────────────────
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // Pobieramy czytelnika wraz z jego wszystkimi wypożyczeniami i tytułami książek
            var czytelnik = await _context.Uzytkownicy
                .Include(u => u.Wypozyczenia)
                    .ThenInclude(w => w.Egzemplarz)
                        .ThenInclude(e => e.Ksiazka)
                .Include(u => u.Kary) // Przy okazji pobierzemy jego kary
                .FirstOrDefaultAsync(u => u.UzytkownikId == id);

            if (czytelnik == null) return NotFound();

            return View(czytelnik);
        }
    }
}