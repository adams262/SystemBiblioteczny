using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemBiblioteczny.Migrations
{
    /// <inheritdoc />
    public partial class AktualizacjaBazy3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CzyAktywna",
                table: "Ksiazki",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CzyAktywna",
                table: "Ksiazki");
        }
    }
}
