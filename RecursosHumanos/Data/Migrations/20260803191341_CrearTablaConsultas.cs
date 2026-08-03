using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecursosHumanos.Data.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaConsultas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "Consulta",
                     columns: table => new {
                         Id = table.Column<int>(type: "int",nullable: false).Annotation( "SqlServer:Identity", "1, 1"),
                         Nombre = table.Column<string>(type: "nvarchar(max)",nullable: false),
                         Sql = table.Column<string>(type: "nvarchar(max)",nullable: true),
                         Activo = table.Column<short>(type: "smallint",nullable: true),
                         Creado = table.Column<DateTime>(type: "datetime2",nullable: false,defaultValueSql: "GETDATE()"),
                         Editado = table.Column<DateTime>(type: "datetime2",nullable: true),
                         Eliminado = table.Column<DateTime>( type: "datetime2", nullable: true)
                     },
                     constraints: table => {
                         table.PrimaryKey("PK_Consultas", x => x.Id);
                     });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            //migrationBuilder.DropForeignKey(name: "FK_Algo_Consultas_Id", table: "OtraTabla");
            migrationBuilder.DropTable(name: "Consulta");
        }
    }
}
