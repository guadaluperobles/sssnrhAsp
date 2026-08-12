using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecursosHumanos.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTipoCheques : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoCheque",
                table: "Cheque",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoCheque",
                table: "Cheque");
        }
    }
}
