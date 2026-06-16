namespace SystemBiblioteczny.Models;

public class Ksiazka
{
    public int KsiazkaId { get; set; }
    public string Tytul { get; set; } = null!;
    public string? ISBN { get; set; }
    public int? RokWydania { get; set; }
    public string? Wydawnictwo { get; set; }
    public int LiczbaEgzemplarzy { get; set; } = 1;

    public bool CzyAktywna { get; set; } = true;

    public int? KategoriaId { get; set; }
    public Kategoria? Kategoria { get; set; }

    public ICollection<KsiazkaAutor> KsiazkaAutorzy { get; set; } = [];
    public ICollection<Egzemplarz> Egzemplarze { get; set; } = [];
    public ICollection<Rezerwacja> Rezerwacje { get; set; } = [];
}
