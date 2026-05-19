using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SystemBiblioteczny.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Autorzy",
                columns: table => new
                {
                    AutorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imie = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nazwisko = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Autorzy", x => x.AutorId);
                });

            migrationBuilder.CreateTable(
                name: "Bibliotekarze",
                columns: table => new
                {
                    BibliotekarzeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imie = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nazwisko = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CzyAktywny = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bibliotekarze", x => x.BibliotekarzeId);
                });

            migrationBuilder.CreateTable(
                name: "Kategorie",
                columns: table => new
                {
                    KategoriaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategorie", x => x.KategoriaId);
                });

            migrationBuilder.CreateTable(
                name: "Uzytkownicy",
                columns: table => new
                {
                    UzytkownikId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imie = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nazwisko = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NumerKarty = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DataRejestracji = table.Column<DateOnly>(type: "date", nullable: false),
                    CzyAktywny = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uzytkownicy", x => x.UzytkownikId);
                });

            migrationBuilder.CreateTable(
                name: "Ksiazki",
                columns: table => new
                {
                    KsiazkaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tytul = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RokWydania = table.Column<int>(type: "int", nullable: true),
                    Wydawnictwo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LiczbaEgzemplarzy = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    KategoriaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ksiazki", x => x.KsiazkaId);
                    table.ForeignKey(
                        name: "FK_Ksiazki_Kategorie_KategoriaId",
                        column: x => x.KategoriaId,
                        principalTable: "Kategorie",
                        principalColumn: "KategoriaId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Egzemplarze",
                columns: table => new
                {
                    EgzemplarzId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Dostepny"),
                    KsiazkaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Egzemplarze", x => x.EgzemplarzId);
                    table.ForeignKey(
                        name: "FK_Egzemplarze_Ksiazki_KsiazkaId",
                        column: x => x.KsiazkaId,
                        principalTable: "Ksiazki",
                        principalColumn: "KsiazkaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KsiazkaAutorzy",
                columns: table => new
                {
                    KsiazkaId = table.Column<int>(type: "int", nullable: false),
                    AutorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KsiazkaAutorzy", x => new { x.KsiazkaId, x.AutorId });
                    table.ForeignKey(
                        name: "FK_KsiazkaAutorzy_Autorzy_AutorId",
                        column: x => x.AutorId,
                        principalTable: "Autorzy",
                        principalColumn: "AutorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KsiazkaAutorzy_Ksiazki_KsiazkaId",
                        column: x => x.KsiazkaId,
                        principalTable: "Ksiazki",
                        principalColumn: "KsiazkaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rezerwacje",
                columns: table => new
                {
                    RezerwacjaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataRezerwacji = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataWaznosci = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Aktywna"),
                    KsiazkaId = table.Column<int>(type: "int", nullable: false),
                    UzytkownikId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezerwacje", x => x.RezerwacjaId);
                    table.ForeignKey(
                        name: "FK_Rezerwacje_Ksiazki_KsiazkaId",
                        column: x => x.KsiazkaId,
                        principalTable: "Ksiazki",
                        principalColumn: "KsiazkaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rezerwacje_Uzytkownicy_UzytkownikId",
                        column: x => x.UzytkownikId,
                        principalTable: "Uzytkownicy",
                        principalColumn: "UzytkownikId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Wypozyczenia",
                columns: table => new
                {
                    WypozyczanieId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataWypozyczenia = table.Column<DateOnly>(type: "date", nullable: false),
                    PlanowanyZwrot = table.Column<DateOnly>(type: "date", nullable: false),
                    DataZwrotu = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Aktywne"),
                    EgzemplarzId = table.Column<int>(type: "int", nullable: false),
                    UzytkownikId = table.Column<int>(type: "int", nullable: false),
                    BibliotekarzeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wypozyczenia", x => x.WypozyczanieId);
                    table.ForeignKey(
                        name: "FK_Wypozyczenia_Bibliotekarze_BibliotekarzeId",
                        column: x => x.BibliotekarzeId,
                        principalTable: "Bibliotekarze",
                        principalColumn: "BibliotekarzeId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Wypozyczenia_Egzemplarze_EgzemplarzId",
                        column: x => x.EgzemplarzId,
                        principalTable: "Egzemplarze",
                        principalColumn: "EgzemplarzId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Wypozyczenia_Uzytkownicy_UzytkownikId",
                        column: x => x.UzytkownikId,
                        principalTable: "Uzytkownicy",
                        principalColumn: "UzytkownikId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Kary",
                columns: table => new
                {
                    KaraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kwota = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    DataNalozenia = table.Column<DateOnly>(type: "date", nullable: false),
                    DataZaplaty = table.Column<DateOnly>(type: "date", nullable: true),
                    Powod = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WypozyczanieId = table.Column<int>(type: "int", nullable: false),
                    UzytkownikId = table.Column<int>(type: "int", nullable: false),
                    BibliotekarzeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kary", x => x.KaraId);
                    table.ForeignKey(
                        name: "FK_Kary_Bibliotekarze_BibliotekarzeId",
                        column: x => x.BibliotekarzeId,
                        principalTable: "Bibliotekarze",
                        principalColumn: "BibliotekarzeId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Kary_Uzytkownicy_UzytkownikId",
                        column: x => x.UzytkownikId,
                        principalTable: "Uzytkownicy",
                        principalColumn: "UzytkownikId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Kary_Wypozyczenia_WypozyczanieId",
                        column: x => x.WypozyczanieId,
                        principalTable: "Wypozyczenia",
                        principalColumn: "WypozyczanieId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Autorzy",
                columns: new[] { "AutorId", "Bio", "Imie", "Nazwisko" },
                values: new object[,]
                {
                    { 1, null, "Andrzej", "Sapkowski" },
                    { 2, null, "Remigiusz", "Mróz" },
                    { 3, null, "Robert C.", "Martin" }
                });

            migrationBuilder.InsertData(
                table: "Kategorie",
                columns: new[] { "KategoriaId", "Nazwa" },
                values: new object[,]
                {
                    { 1, "Fantastyka" },
                    { 2, "Kryminał" },
                    { 3, "Romans" },
                    { 4, "Historia" },
                    { 5, "Informatyka" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bibliotekarze_Email",
                table: "Bibliotekarze",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Egzemplarze_KsiazkaId",
                table: "Egzemplarze",
                column: "KsiazkaId");

            migrationBuilder.CreateIndex(
                name: "IX_Kary_BibliotekarzeId",
                table: "Kary",
                column: "BibliotekarzeId");

            migrationBuilder.CreateIndex(
                name: "IX_Kary_UzytkownikId",
                table: "Kary",
                column: "UzytkownikId");

            migrationBuilder.CreateIndex(
                name: "IX_Kary_WypozyczanieId",
                table: "Kary",
                column: "WypozyczanieId");

            migrationBuilder.CreateIndex(
                name: "IX_Kategorie_Nazwa",
                table: "Kategorie",
                column: "Nazwa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KsiazkaAutorzy_AutorId",
                table: "KsiazkaAutorzy",
                column: "AutorId");

            migrationBuilder.CreateIndex(
                name: "IX_Ksiazki_ISBN",
                table: "Ksiazki",
                column: "ISBN",
                unique: true,
                filter: "[ISBN] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Ksiazki_KategoriaId",
                table: "Ksiazki",
                column: "KategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezerwacje_KsiazkaId",
                table: "Rezerwacje",
                column: "KsiazkaId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezerwacje_UzytkownikId",
                table: "Rezerwacje",
                column: "UzytkownikId");

            migrationBuilder.CreateIndex(
                name: "IX_Uzytkownicy_Email",
                table: "Uzytkownicy",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Uzytkownicy_NumerKarty",
                table: "Uzytkownicy",
                column: "NumerKarty",
                unique: true,
                filter: "[NumerKarty] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Wypozyczenia_BibliotekarzeId",
                table: "Wypozyczenia",
                column: "BibliotekarzeId");

            migrationBuilder.CreateIndex(
                name: "IX_Wypozyczenia_EgzemplarzId",
                table: "Wypozyczenia",
                column: "EgzemplarzId");

            migrationBuilder.CreateIndex(
                name: "IX_Wypozyczenia_UzytkownikId",
                table: "Wypozyczenia",
                column: "UzytkownikId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Kary");

            migrationBuilder.DropTable(
                name: "KsiazkaAutorzy");

            migrationBuilder.DropTable(
                name: "Rezerwacje");

            migrationBuilder.DropTable(
                name: "Wypozyczenia");

            migrationBuilder.DropTable(
                name: "Autorzy");

            migrationBuilder.DropTable(
                name: "Bibliotekarze");

            migrationBuilder.DropTable(
                name: "Egzemplarze");

            migrationBuilder.DropTable(
                name: "Uzytkownicy");

            migrationBuilder.DropTable(
                name: "Ksiazki");

            migrationBuilder.DropTable(
                name: "Kategorie");
        }
    }
}
