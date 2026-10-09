using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecursosHumanos.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEnvioPlantillaEmpleadoEstatal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmpleadoEstatal",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    nemp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    paterno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    materno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    curp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    rfc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sexo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    escol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fecnac = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tipoemp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sind = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    estpza = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pzavac = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    recpza = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    numplaza = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cvepago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fecing = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nivel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    puesto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ptoficial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    mpio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sb = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    quin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    comp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    otp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sbm = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    snm = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    prctrab = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ctdsc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    prctrabdis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ctdsc1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    creado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modificado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    eliminado = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadoEstatal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnvioPlantilla",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Plantilla = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Responsable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    creado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modificado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    eliminado = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvioPlantilla", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpleadoEstatal");

            migrationBuilder.DropTable(
                name: "EnvioPlantilla");
        }
    }
}
