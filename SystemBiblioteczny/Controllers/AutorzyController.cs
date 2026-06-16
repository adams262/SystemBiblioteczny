using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemBiblioteczny.Data;
using SystemBiblioteczny.Models;

namespace SystemBiblioteczny.Controllers
{
    // Dostęp tylko dla bibliotekarza
    [Authorize(Policy = "TylkoBibliotekarze")]
    public class AutorzyController : Controller
    {
        private readonly BibliotekaDbContext _context;

        public AutorzyController(BibliotekaDbContext context)
        {
            _context = context;
        }

        // ─── LISTA ───────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Autorzy.ToListAsync());
        }

        // ─── CREATE (GET) ────────────────────────────────────────
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // ─── CREATE (POST) ───────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Autor autor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(autor);
                await _context.SaveChangesAsync();
                TempData["Sukces"] = "Autor został dodany.";
                return RedirectToAction(nameof(Index));
            }
            return View(autor);
        }

        // ─── DELETE (GET) ────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var autor = await _context.Autorzy.FirstOrDefaultAsync(a => a.AutorId == id);
            if (autor == null) return NotFound();

            return View(autor);
        }

        // ─── DELETE (POST) ───────────────────────────────────────
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var autor = await _context.Autorzy.FindAsync(id);
            if (autor != null)
            {
                // Zabezpieczenie przed usunięciem autora, który ma przypisane książki
                bool maKsiazki = await _context.KsiazkaAutorzy.AnyAsync(ka => ka.AutorId == id);

                if (maKsiazki)
                {
                    TempData["Blad"] = "Nie można usunąć autora, ponieważ posiada przypisane książki!";
                    return RedirectToAction(nameof(Index));
                }

                _context.Autorzy.Remove(autor);
                await _context.SaveChangesAsync();
                TempData["Sukces"] = "Autor został usunięty.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}