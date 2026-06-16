using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemBiblioteczny.Data;
using SystemBiblioteczny.Models;

namespace SystemBiblioteczny.Controllers
{
    [Authorize(Policy = "TylkoBibliotekarze")] // Dostęp tylko dla zalogowanych bibliotekarzy
    public class KategorieController : Controller
    {
        private readonly BibliotekaDbContext _context;

        public KategorieController(BibliotekaDbContext context)
        {
            _context = context;
        }

        // ─── 1. WYŚWIETLANIE (Lista) ──────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var kategorie = await _context.Kategorie.ToListAsync();
            return View(kategorie);
        }

        // ─── 2. DODAWANIE (Formularz - GET) ───────────────────────
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // ─── 3. DODAWANIE (Zapis do bazy - POST) ──────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Kategoria kategoria)
        {
            if (!ModelState.IsValid) return View(kategoria);

            bool istnieje = await _context.Kategorie
                .AnyAsync(k => k.Nazwa.ToLower() == kategoria.Nazwa.ToLower());

            if (istnieje)
            {
                ModelState.AddModelError("Nazwa", "Taka kategoria już istnieje.");
                return View(kategoria);
            }

            _context.Kategorie.Add(kategoria);
            await _context.SaveChangesAsync();

            // Używamy unikalnego klucza
            TempData["SukcesKategoria"] = "Kategoria została dodana pomyślnie.";
            return RedirectToAction(nameof(Index));
        }

        // ─── 4. USUWANIE (Potwierdzenie - GET) ────────────────────
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var kategoria = await _context.Kategorie
                .FirstOrDefaultAsync(k => k.KategoriaId == id);

            if (kategoria == null)
                return NotFound();

            return View(kategoria);
        }

        // ─── 5. USUWANIE (Wykonanie - POST) ───────────────────────
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kategoria = await _context.Kategorie.FindAsync(id);
            if (kategoria != null)
            {
                bool maKsiazki = await _context.Ksiazki.AnyAsync(k => k.KategoriaId == id);

                if (maKsiazki)
                {
                    // Używamy unikalnego klucza
                    TempData["BladKategoria"] = "Nie można usunąć tej kategorii, ponieważ są do niej przypisane książki!";
                    return RedirectToAction(nameof(Index));
                }

                _context.Kategorie.Remove(kategoria);
                await _context.SaveChangesAsync();
                TempData["SukcesKategoria"] = "Kategoria została pomyślnie usunięta.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}