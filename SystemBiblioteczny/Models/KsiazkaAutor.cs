namespace SystemBiblioteczny.Models;

public class KsiazkaAutor
{
    public int KsiazkaId { get; set; }
    public Ksiazka Ksiazka { get; set; } = null!;

    public int AutorId { get; set; }
    public Autor Autor { get; set; } = null!;
}
