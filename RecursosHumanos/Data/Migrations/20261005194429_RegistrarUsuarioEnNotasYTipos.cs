using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecursosHumanos.Data.Migrations
{
    /// <inheritdoc />
    public partial class RegistrarUsuarioEnNotasYTipos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreadoPor",
                table: "TipoNota",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditadoPor",
                table: "TipoNota",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EliminadoPor",
                table: "TipoNota",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreadoPor",
                table: "Nota",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditadoPor",
                table: "Nota",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EliminadoPor",
                table: "Nota",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Fecha",
                table: "Nota",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreadoPor",
                table: "TipoNota");

            migrationBuilder.DropColumn(
                name: "EditadoPor",
                table: "TipoNota");

            migrationBuilder.DropColumn(
                name: "EliminadoPor",
                table: "TipoNota");

            migrationBuilder.DropColumn(
                name: "CreadoPor",
                table: "Nota");

            migrationBuilder.DropColumn(
                name: "EditadoPor",
                table: "Nota");

            migrationBuilder.DropColumn(
                name: "EliminadoPor",
                table: "Nota");

            migrationBuilder.DropColumn(
                name: "Fecha",
                table: "Nota");
        }
    }
}
