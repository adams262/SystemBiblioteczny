namespace SystemBiblioteczny.Models
{
    public class Kategoria
    {
        public int KategoriaId { get; set; }
        public string Nazwa { get; set; } = null!;

        public ICollection<Ksiazka> Ksiazki { get; set; } = [];
    }
}
