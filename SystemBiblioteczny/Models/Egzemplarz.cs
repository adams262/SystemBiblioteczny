namespace SystemBiblioteczny.Models;

public enum StatusEgzemplarza
{
    Dostepny,
    Wypozyczony,
    Zarezerwowany,
    Niedostepny
}

public class Egzemplarz
{
    public int EgzemplarzId { get; set; }
    public StatusEgzemplarza Status { get; set; } = StatusEgzemplarza.Dostepny;

    public int KsiazkaId { get; set; }
    public Ksiazka Ksiazka { get; set; } = null!;

    public ICollection<Wypozyczenie> Wypozyczenia { get; set; } = [];
}
