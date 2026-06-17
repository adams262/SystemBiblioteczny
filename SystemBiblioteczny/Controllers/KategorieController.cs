using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemBiblioteczny.Data;
using SystemBiblioteczny.Models;

namespace SystemBiblioteczny.Controllers
{

    // Dostęp tylko dla bibliotekarzy

    [Authorize(Policy = "TylkoBibliotekarze")] 
    public class KategorieController : Controller
    {
        private readonly BibliotekaDbContext _context;

        public KategorieController(BibliotekaDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var kategorie = await _context.Kategorie.ToListAsync();
            return View(kategorie);
        }

        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        
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

           
            TempData["SukcesKategoria"] = "Kategoria została dodana pomyślnie.";
            return RedirectToAction(nameof(Index));
        }

        
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