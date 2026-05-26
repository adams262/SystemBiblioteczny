using Microsoft.AspNetCore.Mvc.Rendering;

namespace SystemBiblioteczny.Models.ViewModels
{
    public class KsiazkaCreateViewModel
    {
        public string Tytul { get; set; } = null!;
        public string? ISBN { get; set; }
        public int? RokWydania { get; set; }
        public string? Wydawnictwo { get; set; }

        public int? KategoriaId { get; set; }

        // WYBRANI AUTORZY
        public List<int> WybraniAutorzy { get; set; } = [];

        // LISTA AUTORÓW DO SELECTA
        public MultiSelectList? Autorzy { get; set; }
    }
}