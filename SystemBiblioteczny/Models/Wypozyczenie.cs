namespace SystemBiblioteczny.Models;

public enum StatusWypozyczenia
{
    Aktywne,
    Zwrocone,
    Przeterminowane
}

public class Wypozyczenie
{
    public int WypozyczanieId { get; set; }
    public DateOnly DataWypozyczenia { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly PlanowanyZwrot { get; set; }
    public DateOnly? DataZwrotu { get; set; }
    public StatusWypozyczenia Status { get; set; } = StatusWypozyczenia.Aktywne;

    public int LiczbaPrzedluzen { get; set; } = 0;

    public int EgzemplarzId { get; set; }
    public Egzemplarz Egzemplarz { get; set; } = null!;

    public int UzytkownikId { get; set; }
    public Uzytkownik Uzytkownik { get; set; } = null!;

    public int? BibliotekarzeId { get; set; }
    public Bibliotekarz? Bibliotekarz { get; set; }

    public ICollection<Kara> Kary { get; set; } = [];
}
