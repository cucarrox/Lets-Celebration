using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoFinalEntra21.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlteracaoTamanhoMensagens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Corpo",
                table: "Mensagens",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Corpo",
                table: "Mensagens",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
