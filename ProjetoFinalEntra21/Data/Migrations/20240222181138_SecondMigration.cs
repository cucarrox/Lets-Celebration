using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoFinalEntra21.Data.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "Eventos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_StatusId",
                table: "Eventos",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_StatusEventos_StatusId",
                table: "Eventos",
                column: "StatusId",
                principalTable: "StatusEventos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_StatusEventos_StatusId",
                table: "Eventos");

            migrationBuilder.DropIndex(
                name: "IX_Eventos_StatusId",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Eventos");
        }
    }
}
