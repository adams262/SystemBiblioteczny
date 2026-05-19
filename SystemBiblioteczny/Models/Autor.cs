namespace SystemBiblioteczny.Models;

public class Autor
{
    public int AutorId { get; set; }
    public string Imie { get; set; } = null!;
    public string Nazwisko { get; set; } = null!;
    public string? Bio { get; set; }

    // Nawigacja
    public ICollection<KsiazkaAutor> KsiazkaAutorzy { get; set; } = [];
}
