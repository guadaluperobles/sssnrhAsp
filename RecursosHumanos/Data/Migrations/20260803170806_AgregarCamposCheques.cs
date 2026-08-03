using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecursosHumanos.Data.Migrations {
    /// <inheritdoc />
    public partial class AgregarCamposCheques : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.AddColumn<DateTime>(
                name: "Creado",
                table: "Cheque",
                nullable: false,
                defaultValue: DateTime.MinValue);

            migrationBuilder.AddColumn<DateTime>(
                name: "Editado",
                table: "Cheque",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Eliminado",
                table: "Cheque",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Impreso",
                table: "Cheque",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropColumn("Creado", "Cheque");
            migrationBuilder.DropColumn("Editado", "Cheque");
            migrationBuilder.DropColumn("Eliminado", "Cheque");
            migrationBuilder.DropColumn("Impreso", "Cheque");

        }
    }
}
