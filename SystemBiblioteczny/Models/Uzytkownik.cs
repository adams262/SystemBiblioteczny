namespace SystemBiblioteczny.Models;

public class Uzytkownik
{
    public int UzytkownikId { get; set; }
    public string Imie { get; set; } = null!;
    public string Nazwisko { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = string.Empty;
    public string? NumerKarty { get; set; }
    public DateTime DataRejestracji { get; set; } = DateTime.Now;
    public bool CzyAktywny { get; set; } = true;

    public ICollection<Wypozyczenie> Wypozyczenia { get; set; } = [];
    public ICollection<Rezerwacja> Rezerwacje { get; set; } = [];
    public ICollection<Kara> Kary { get; set; } = [];
}
