namespace SystemBiblioteczny.Models;

public enum StatusRezerwacji
{
    Aktywna,
    Zrealizowana,
    Anulowana,
    Wygasla
}

public class Rezerwacja
{
    public int RezerwacjaId { get; set; }
    public DateTime DataRezerwacji { get; set; } = DateTime.Now;
    public DateOnly DataWaznosci { get; set; }
    public StatusRezerwacji Status { get; set; } = StatusRezerwacji.Aktywna;

    public int KsiazkaId { get; set; }
    public Ksiazka Ksiazka { get; set; } = null!;

    public int UzytkownikId { get; set; }
    public Uzytkownik Uzytkownik { get; set; } = null!;
}
