using SystemBiblioteczny.Models;

namespace SystemBiblioteczny.Models.ViewModels
{
    public class KsiazkaIndexViewModel
    {
        public List<Ksiazka> Ksiazki { get; set; } = [];

        public string? SzukajTytul { get; set; }

        public string? SzukajISBN { get; set; }

        public string? SzukajAutor { get; set; }

        public int? KategoriaId { get; set; }

        public List<Kategoria> Kategorie { get; set; } = [];
    }
}