using System.ComponentModel.DataAnnotations;

namespace SystemBiblioteczny.Models.ViewModels
{
    // ─── LOGIN ───────────────────────────────────────────────────

    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-mail jest wymagany.")]
        [EmailAddress(ErrorMessage = "Podaj prawidłowy adres e-mail.")]
        [Display(Name = "Adres e-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Haslo { get; set; } = string.Empty;
    }

    // ─── REJESTRACJA CZYTELNIKA ──────────────────────────────────

    public class RejestracjaCzytelnikaViewModel
    {
        [Required(ErrorMessage = "Imię jest wymagane.")]
        [StringLength(50)]
        [Display(Name = "Imię")]
        public string Imie { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        [StringLength(100)]
        [Display(Name = "Nazwisko")]
        public string Nazwisko { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-mail jest wymagany.")]
        [EmailAddress(ErrorMessage = "Podaj prawidłowy adres e-mail.")]
        [Display(Name = "Adres e-mail")]
        public string Email { get; set; } = string.Empty;
      

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Hasło musi mieć co najmniej 6 znaków.")]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Haslo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane.")]
        [DataType(DataType.Password)]
        [Compare("Haslo", ErrorMessage = "Hasła nie są identyczne.")]
        [Display(Name = "Potwierdź hasło")]
        public string PotwierdzHaslo { get; set; } = string.Empty;
    }

    // ─── REJESTRACJA BIBLIOTEKARZA ───────────────────────────────

    public class RejestracjaBibliotekarzeViewModel
    {
        [Required(ErrorMessage = "Imię jest wymagane.")]
        [StringLength(50)]
        [Display(Name = "Imię")]
        public string Imie { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        [StringLength(100)]
        [Display(Name = "Nazwisko")]
        public string Nazwisko { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-mail jest wymagany.")]
        [EmailAddress(ErrorMessage = "Podaj prawidłowy adres e-mail.")]
        [Display(Name = "Adres e-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Hasło musi mieć co najmniej 6 znaków.")]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Haslo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane.")]
        [DataType(DataType.Password)]
        [Compare("Haslo", ErrorMessage = "Hasła nie są identyczne.")]
        [Display(Name = "Potwierdź hasło")]
        public string PotwierdzHaslo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kod dostępu jest wymagany.")]
        [Display(Name = "Kod dostępu bibliotekarza")]
        public string KodDostepu { get; set; } = string.Empty;
    }
}
