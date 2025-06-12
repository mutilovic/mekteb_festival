using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mekteb_Festival.Migrations
{
    /// <inheritdoc />
    public partial class RenameFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ime",
                table: "Takmicari");

            migrationBuilder.DropColumn(
                name: "Ime",
                table: "Registrations");

            migrationBuilder.RenameColumn(
                name: "Prezime",
                table: "Takmicari",
                newName: "ImePrezime");

            migrationBuilder.RenameColumn(
                name: "Prezime",
                table: "Registrations",
                newName: "ImePrezime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImePrezime",
                table: "Takmicari",
                newName: "Prezime");

            migrationBuilder.RenameColumn(
                name: "ImePrezime",
                table: "Registrations",
                newName: "Prezime");

            migrationBuilder.AddColumn<string>(
                name: "Ime",
                table: "Takmicari",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ime",
                table: "Registrations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
