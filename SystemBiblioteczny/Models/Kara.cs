namespace SystemBiblioteczny.Models;

public class Kara
{
    public int KaraId { get; set; }
    public decimal Kwota { get; set; }
    public DateOnly DataNalozenia { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly? DataZaplaty { get; set; }
    public string? Powod { get; set; }

    public int WypozyczanieId { get; set; }
    public Wypozyczenie Wypozyczenie { get; set; } = null!;

    public int UzytkownikId { get; set; }
    public Uzytkownik Uzytkownik { get; set; } = null!;

    public int? BibliotekarzeId { get; set; }
    public Bibliotekarz? Bibliotekarz { get; set; }
}
