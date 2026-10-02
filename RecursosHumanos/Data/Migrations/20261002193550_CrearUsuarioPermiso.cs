using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecursosHumanos.Data.Migrations
{
    /// <inheritdoc />
    public partial class CrearUsuarioPermiso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsuarioPermiso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermisoId = table.Column<int>(type: "int", nullable: true),
                    Creado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Editado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Eliminado = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioPermiso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioPermiso_PermisoVistaModel_PermisoId",
                        column: x => x.PermisoId,
                        principalTable: "PermisoVistaModel",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPermiso_PermisoId",
                table: "UsuarioPermiso",
                column: "PermisoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuarioPermiso");
        }
    }
}
