using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SystemBiblioteczny.Data;
using SystemBiblioteczny.Models;
using SystemBiblioteczny.Models.ViewModels;

namespace SystemBiblioteczny.Controllers
{
    public class KsiazkiController : Controller
    {
        private readonly BibliotekaDbContext _context;

        public KsiazkiController(BibliotekaDbContext context)
        {
            _context = context;
        }

        // LISTA
        public async Task<IActionResult> Index(
    string? szukajTytul,
    string? szukajISBN,
    int? szukanyAutorId,
    int? kategoriaId)
        {
            var query = _context.Ksiazki
                .Where(k => k.CzyAktywna == true)
                .Include(k => k.Kategoria)
                .Include(k => k.KsiazkaAutorzy)
                    .ThenInclude(ka => ka.Autor)
                .AsQueryable();

            // TYTUŁ
            if (!string.IsNullOrWhiteSpace(szukajTytul))
            {
                query = query.Where(k =>
                    k.Tytul.Contains(szukajTytul));
            }

            // ISBN
            if (!string.IsNullOrWhiteSpace(szukajISBN))
            {
                query = query.Where(k =>
                    k.ISBN != null &&
                    k.ISBN.Contains(szukajISBN));
            }

            // AUTOR


            if (szukanyAutorId.HasValue)
            {
                query = query.Where(k => k.KsiazkaAutorzy.Any(ka => ka.AutorId == szukanyAutorId));
            }

            if (kategoriaId.HasValue)
            {
                query = query.Where(k => k.KategoriaId == kategoriaId.Value);
            }
            // Przygotowanie listy autorów do widoku
            ViewBag.ListaAutorow = new SelectList(
                _context.Autorzy.Select(a => new {
                    Id = a.AutorId,
                    Nazwa = a.Imie + " " + a.Nazwisko
                }).ToList(),
                "Id",
                "Nazwa"
            );

            var vm = new KsiazkaIndexViewModel
            {
                Ksiazki = await query.ToListAsync(),
                SzukajTytul = szukajTytul,
                SzukajISBN = szukajISBN,
                SzukanyAutorId = szukanyAutorId, 
                KategoriaId = kategoriaId,
                Kategorie = await _context.Kategorie.ToListAsync()
            };

            return View(vm);
        }

        // SZCZEGÓŁY
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var ksiazka = await _context.Ksiazki
                .Include(k => k.Kategoria)
                .Include(k => k.KsiazkaAutorzy)
                    .ThenInclude(ka => ka.Autor)
                .FirstOrDefaultAsync(m => m.KsiazkaId == id);

            if (ksiazka == null)
                return NotFound();

            return View(ksiazka);
        }

        // CREATE GET
        public IActionResult Create()
        {
            var vm = new KsiazkaCreateViewModel
            {
                Autorzy = new MultiSelectList(
                    _context.Autorzy.ToList(),
                    "AutorId",
                    "Nazwisko"
)
            };

            ViewBag.Kategorie = new SelectList(
                _context.Kategorie,
                "KategoriaId",
                "Nazwa"
            );

            return View(vm);
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KsiazkaCreateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var ksiazka = new Ksiazka
                {
                    Tytul = vm.Tytul,
                    ISBN = vm.ISBN,
                    RokWydania = vm.RokWydania,
                    Wydawnictwo = vm.Wydawnictwo,
                    KategoriaId = vm.KategoriaId,
                    LiczbaEgzemplarzy = vm.LiczbaEgzemplarzy
                };

                _context.Ksiazki.Add(ksiazka);

                await _context.SaveChangesAsync();

                for (int i = 0; i < ksiazka.LiczbaEgzemplarzy; i++)
                {
                    _context.Egzemplarze.Add(new Egzemplarz
                    {
                        KsiazkaId = ksiazka.KsiazkaId,
                        Status = StatusEgzemplarza.Dostepny
                    });
                }

                await _context.SaveChangesAsync();

                // RELACJA M:N
                _context.KsiazkaAutorzy.Add(new KsiazkaAutor
                {
                    KsiazkaId = ksiazka.KsiazkaId,
                    AutorId = vm.AutorId
                });

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            vm.Autorzy = new MultiSelectList(
                _context.Autorzy.ToList(),
                "AutorId",
                "Nazwisko"
            );

            ViewBag.Kategorie = new SelectList(
                _context.Kategorie,
                "KategoriaId",
                "Nazwa"
            );

            return View(vm);
        }

        // EDIT GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var ksiazka = await _context.Ksiazki
                .Include(k => k.KsiazkaAutorzy)
                .FirstOrDefaultAsync(k => k.KsiazkaId == id);

            if (ksiazka == null)
                return NotFound();

            var autorId = ksiazka.KsiazkaAutorzy
                .FirstOrDefault()?.AutorId;

            var vm = new KsiazkaCreateViewModel
            {
                Tytul = ksiazka.Tytul,
                ISBN = ksiazka.ISBN,
                RokWydania = ksiazka.RokWydania,
                Wydawnictwo = ksiazka.Wydawnictwo,
                KategoriaId = ksiazka.KategoriaId,
                AutorId = autorId ?? 0,

                Autorzy = new MultiSelectList(
                    _context.Autorzy.ToList(),
                    "AutorId",
                    "Nazwisko"
                )
            };

            ViewBag.Kategorie = new SelectList(
                _context.Kategorie,
                "KategoriaId",
                "Nazwa",
                ksiazka.KategoriaId
            );

            ViewBag.KsiazkaId = ksiazka.KsiazkaId;

            return View(vm);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KsiazkaCreateViewModel vm)
        {
            var ksiazka = await _context.Ksiazki
                .Include(k => k.KsiazkaAutorzy)
                .FirstOrDefaultAsync(k => k.KsiazkaId == id);

            if (ksiazka == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                ksiazka.Tytul = vm.Tytul;
                ksiazka.ISBN = vm.ISBN;
                ksiazka.RokWydania = vm.RokWydania;
                ksiazka.Wydawnictwo = vm.Wydawnictwo;
                ksiazka.KategoriaId = vm.KategoriaId;

                // USUNIĘCIE STAREGO AUTORA
                _context.KsiazkaAutorzy.RemoveRange(
                    ksiazka.KsiazkaAutorzy);

                // DODANIE NOWEGO
                _context.KsiazkaAutorzy.Add(new KsiazkaAutor
                {
                    KsiazkaId = ksiazka.KsiazkaId,
                    AutorId = vm.AutorId
                });

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            vm.Autorzy = new MultiSelectList(
                _context.Autorzy.ToList(),
                "AutorId",
                "Nazwisko"
            );

            ViewBag.Kategorie = new SelectList(
                _context.Kategorie,
                "KategoriaId",
                "Nazwa",
                vm.KategoriaId
            );

            return View(vm);
        }

        // DELETE GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var ksiazka = await _context.Ksiazki
                .Include(k => k.Kategoria)
                .FirstOrDefaultAsync(m => m.KsiazkaId == id);

            if (ksiazka == null)
                return NotFound();

            return View(ksiazka);
        }

        // DELETE POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ksiazka = await _context.Ksiazki.FindAsync(id);

            if (ksiazka != null)
            {
                
                ksiazka.CzyAktywna = false;

                
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DodajEgzemplarz(int id)
        {
            var ksiazka = await _context.Ksiazki.FindAsync(id);
            if (ksiazka == null) return NotFound();

            var nowyEgzemplarz = new Egzemplarz
            {
                KsiazkaId = ksiazka.KsiazkaId,
                Status = StatusEgzemplarza.Dostepny
            };

            _context.Egzemplarze.Add(nowyEgzemplarz);
            ksiazka.LiczbaEgzemplarzy++;
            await _context.SaveChangesAsync();

            
            TempData["SukcesEgzemplarz"] = "Pomyślnie dodano nowy egzemplarz do systemu!";

            return RedirectToAction(nameof(Egzemplarze), new { id = ksiazka.KsiazkaId });
        }

        public async Task<IActionResult> Egzemplarze(int id)
        {
            var ksiazka = await _context.Ksiazki
                .Include(k => k.Egzemplarze)
                .FirstOrDefaultAsync(k => k.KsiazkaId == id);

            if (ksiazka == null)
                return NotFound();

            return View(ksiazka);
        }
    }


}