    using Microsoft.EntityFrameworkCore;
    using SystemBiblioteczny.Models;


    namespace SystemBiblioteczny.Data;

    public class BibliotekaDbContext : DbContext
    {
        public BibliotekaDbContext(DbContextOptions<BibliotekaDbContext> options)
            : base(options) { }

        // ── DbSety ────────────────────────────────────────────────────────────
        public DbSet<Autor> Autorzy { get; set; }
        public DbSet<Kategoria> Kategorie { get; set; }
        public DbSet<Ksiazka> Ksiazki { get; set; }
        public DbSet<KsiazkaAutor> KsiazkaAutorzy { get; set; }
        public DbSet<Uzytkownik> Uzytkownicy { get; set; }
        public DbSet<Bibliotekarz> Bibliotekarze { get; set; }
        public DbSet<Egzemplarz> Egzemplarze { get; set; }
        public DbSet<Wypozyczenie> Wypozyczenia { get; set; }
        public DbSet<Rezerwacja> Rezerwacje { get; set; }
        public DbSet<Kara> Kary { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Autor ─────────────────────────────────────────────────────────
            modelBuilder.Entity<Autor>(e =>
            {
                e.HasKey(a => a.AutorId);
                e.Property(a => a.Imie).HasMaxLength(100).IsRequired();
                e.Property(a => a.Nazwisko).HasMaxLength(100).IsRequired();
                e.Property(a => a.Bio).HasMaxLength(1000);
            });

            // ── Kategoria ─────────────────────────────────────────────────────
            modelBuilder.Entity<Kategoria>(e =>
            {
                e.HasKey(k => k.KategoriaId);
                e.Property(k => k.Nazwa).HasMaxLength(100).IsRequired();
                e.HasIndex(k => k.Nazwa).IsUnique();
            });

            // ── Ksiazka ───────────────────────────────────────────────────────
            modelBuilder.Entity<Ksiazka>(e =>
            {
                e.HasKey(k => k.KsiazkaId);
                e.Property(k => k.Tytul).HasMaxLength(300).IsRequired();
                e.Property(k => k.ISBN).HasMaxLength(20);
                e.HasIndex(k => k.ISBN).IsUnique();
                e.Property(k => k.Wydawnictwo).HasMaxLength(200);
                e.Property(k => k.LiczbaEgzemplarzy).HasDefaultValue(1);

                e.HasOne(k => k.Kategoria)
                 .WithMany(kat => kat.Ksiazki)
                 .HasForeignKey(k => k.KategoriaId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // ── KsiazkaAutor (klucz złożony, relacja M:N) ─────────────────────
            modelBuilder.Entity<KsiazkaAutor>(e =>
            {
                e.HasKey(ka => new { ka.KsiazkaId, ka.AutorId });

                e.HasOne(ka => ka.Ksiazka)
                 .WithMany(k => k.KsiazkaAutorzy)
                 .HasForeignKey(ka => ka.KsiazkaId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(ka => ka.Autor)
                 .WithMany(a => a.KsiazkaAutorzy)
                 .HasForeignKey(ka => ka.AutorId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Uzytkownik ────────────────────────────────────────────────────
            modelBuilder.Entity<Uzytkownik>(e =>
            {
                e.HasKey(u => u.UzytkownikId);
                e.Property(u => u.Imie).HasMaxLength(100).IsRequired();
                e.Property(u => u.Nazwisko).HasMaxLength(100).IsRequired();
                e.Property(u => u.Email).HasMaxLength(200).IsRequired();
                e.HasIndex(u => u.Email).IsUnique();
                e.Property(u => u.PasswordHash).HasMaxLength(500).IsRequired();
                e.Property(u => u.NumerKarty).HasMaxLength(50);
                e.HasIndex(u => u.NumerKarty).IsUnique();
                e.Property(u => u.CzyAktywny).HasDefaultValue(true);
            });

            // ── Bibliotekarz ──────────────────────────────────────────────────
            modelBuilder.Entity<Bibliotekarz>(e =>
            {
                e.HasKey(b => b.BibliotekarzeId);
                e.Property(b => b.Imie).HasMaxLength(100).IsRequired();
                e.Property(b => b.Nazwisko).HasMaxLength(100).IsRequired();
                e.Property(b => b.Email).HasMaxLength(200).IsRequired();
                e.HasIndex(b => b.Email).IsUnique();
                e.Property(b => b.PasswordHash).HasMaxLength(500).IsRequired();
                e.Property(b => b.CzyAktywny).HasDefaultValue(true);
            });

            // ── Egzemplarz ────────────────────────────────────────────────────
            modelBuilder.Entity<Egzemplarz>(e =>
            {
                e.HasKey(eg => eg.EgzemplarzId);

                // Enum zapisywany jako string (czytelniejsze w bazie)
                e.Property(eg => eg.Status)
                 .HasConversion<string>()
                 .HasMaxLength(30)
                 .HasDefaultValue(StatusEgzemplarza.Dostepny);

                e.HasOne(eg => eg.Ksiazka)
                 .WithMany(k => k.Egzemplarze)
                 .HasForeignKey(eg => eg.KsiazkaId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Wypozyczenie ──────────────────────────────────────────────────
            modelBuilder.Entity<Wypozyczenie>(e =>
            {
                e.HasKey(w => w.WypozyczanieId);

                e.Property(w => w.Status)
                 .HasConversion<string>()
                 .HasMaxLength(20)
                 .HasDefaultValue(StatusWypozyczenia.Aktywne);

                e.HasOne(w => w.Egzemplarz)
                 .WithMany(eg => eg.Wypozyczenia)
                 .HasForeignKey(w => w.EgzemplarzId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(w => w.Uzytkownik)
                 .WithMany(u => u.Wypozyczenia)
                 .HasForeignKey(w => w.UzytkownikId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(w => w.Bibliotekarz)
                 .WithMany(b => b.Wypozyczenia)
                 .HasForeignKey(w => w.BibliotekarzeId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // ── Rezerwacja ────────────────────────────────────────────────────
            modelBuilder.Entity<Rezerwacja>(e =>
            {
                e.HasKey(r => r.RezerwacjaId);

                e.Property(r => r.Status)
                 .HasConversion<string>()
                 .HasMaxLength(20)
                 .HasDefaultValue(StatusRezerwacji.Aktywna);

                e.HasOne(r => r.Ksiazka)
                 .WithMany(k => k.Rezerwacje)
                 .HasForeignKey(r => r.KsiazkaId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(r => r.Uzytkownik)
                 .WithMany(u => u.Rezerwacje)
                 .HasForeignKey(r => r.UzytkownikId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ── Kara ──────────────────────────────────────────────────────────
            modelBuilder.Entity<Kara>(e =>
            {
                e.HasKey(k => k.KaraId);
                e.Property(k => k.Kwota).HasColumnType("decimal(8,2)").IsRequired();
                e.Property(k => k.Powod).HasMaxLength(500);

                e.HasOne(k => k.Wypozyczenie)
                 .WithMany(w => w.Kary)
                 .HasForeignKey(k => k.WypozyczanieId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(k => k.Uzytkownik)
                 .WithMany(u => u.Kary)
                 .HasForeignKey(k => k.UzytkownikId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(k => k.Bibliotekarz)
                 .WithMany(b => b.Kary)
                 .HasForeignKey(k => k.BibliotekarzeId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // ── Dane seedowe ──────────────────────────────────────────────────
            modelBuilder.Entity<Kategoria>().HasData(
                new Kategoria { KategoriaId = 1, Nazwa = "Fantastyka" },
                new Kategoria { KategoriaId = 2, Nazwa = "Kryminał" },
                new Kategoria { KategoriaId = 3, Nazwa = "Romans" },
                new Kategoria { KategoriaId = 4, Nazwa = "Historia" },
                new Kategoria { KategoriaId = 5, Nazwa = "Informatyka" }
            );

            modelBuilder.Entity<Autor>().HasData(
                new Autor { AutorId = 1, Imie = "Andrzej", Nazwisko = "Sapkowski" },
                new Autor { AutorId = 2, Imie = "Remigiusz", Nazwisko = "Mróz" },
                new Autor { AutorId = 3, Imie = "Robert C.", Nazwisko = "Martin" }
            );
        }
    }
