using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemBiblioteczny.Migrations
{
    /// <inheritdoc />
    public partial class AktualizacjaBazy2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LiczbaPrzedluzen",
                table: "Wypozyczenia",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "CzyOplacona",
                table: "Kary",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LiczbaPrzedluzen",
                table: "Wypozyczenia");

            migrationBuilder.DropColumn(
                name: "CzyOplacona",
                table: "Kary");
        }
    }
}
