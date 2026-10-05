using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecursosHumanos.Data.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaNotasYTipos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TipoNota",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ColorR = table.Column<int>(type: "int", nullable: false),
                    ColorG = table.Column<int>(type: "int", nullable: false),
                    ColorB = table.Column<int>(type: "int", nullable: false),
                    Creado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Editado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Eliminado = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoNota", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Nota",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoNotaId = table.Column<int>(type: "int", nullable: false),
                    EsParaTodos = table.Column<bool>(type: "bit", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Creado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Editado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Eliminado = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nota", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nota_TipoNota_TipoNotaId",
                        column: x => x.TipoNotaId,
                        principalTable: "TipoNota",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Nota_TipoNotaId",
                table: "Nota",
                column: "TipoNotaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Nota");

            migrationBuilder.DropTable(
                name: "TipoNota");
        }
    }
}
