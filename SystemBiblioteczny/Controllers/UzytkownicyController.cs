using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemBiblioteczny.Data;

namespace SystemBiblioteczny.Controllers
{
    [Authorize(Policy = "TylkoBibliotekarze")] 
    public class UzytkownicyController : Controller
    {
        private readonly BibliotekaDbContext _context;

        public UzytkownicyController(BibliotekaDbContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index()
        {
            // wypozyczenia czytelnikow
            var czytelnicy = await _context.Uzytkownicy
                .Include(u => u.Wypozyczenia)
                .ToListAsync();

            return View(czytelnicy);
        }

        // szczegoly czytelnika i jego ksiazki
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            
            var czytelnik = await _context.Uzytkownicy
                .Include(u => u.Wypozyczenia)
                    .ThenInclude(w => w.Egzemplarz)
                        .ThenInclude(e => e.Ksiazka)
                .Include(u => u.Kary) 
                .FirstOrDefaultAsync(u => u.UzytkownikId == id);

            if (czytelnik == null) return NotFound();

            return View(czytelnik);
        }
    }
}