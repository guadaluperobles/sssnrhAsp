using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
namespace RecursosHumanos.Data.Migrations {
    /// <inheritdoc />
    public partial class CrearTablaCheque : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "Cheque",
                columns: table => new {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    ClkDet = table.Column<int>(type: "int", nullable: true),
                    NombreEmpleado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NombreBeneficiario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RfcEmpleado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClavePresupuestal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroCheque = table.Column<int>(type: "int", nullable: true),
                    ClaveUbicacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InicioPeriodo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinPeriodo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Neto = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Deducciones = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    VPA = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    VPATexto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    impreso = table.Column<short>(type: "smallint", nullable: true),
                    Ejercicio = table.Column<int>(type: "int", nullable: true),
                    Quincena = table.Column<int>(type: "int", nullable: true),
                    Creado = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Editado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Eliminado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Impresion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Cheque", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(name: "Cheque");
        }
    }
}
