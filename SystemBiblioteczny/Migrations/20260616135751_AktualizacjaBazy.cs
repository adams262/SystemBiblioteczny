using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemBiblioteczny.Migrations
{
    /// <inheritdoc />
    public partial class AktualizacjaBazy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DataRejestracji",
                table: "Uzytkownicy",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataZatrudnienia",
                table: "Bibliotekarze",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataZatrudnienia",
                table: "Bibliotekarze");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DataRejestracji",
                table: "Uzytkownicy",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
