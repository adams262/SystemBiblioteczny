using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemBiblioteczny.Data;

public class KaryController : Controller
{
    private readonly BibliotekaDbContext _context;

    public KaryController(BibliotekaDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var kary = await _context.Kary
            .Include(k => k.Uzytkownik)
            .ToListAsync();

        return View(kary);
    }

    public async Task<IActionResult> Oplac(int id)
    {
        var kara = await _context.Kary.FindAsync(id);

        if (kara == null)
            return NotFound();

        kara.DataZaplaty = DateOnly.FromDateTime(DateTime.Today);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}