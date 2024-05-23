using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProjetoFinalEntra21.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlteracaoNomeRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioEvento_TipoUsuarios_TipoUsuariosId",
                table: "UsuarioEvento");

            migrationBuilder.DropTable(
                name: "TipoUsuarios");

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tipo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "Tipo" },
                values: new object[,]
                {
                    { 1, "Administrador" },
                    { 2, "Convidado" },
                    { 3, "Destinatário" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioEvento_roles_TipoUsuariosId",
                table: "UsuarioEvento",
                column: "TipoUsuariosId",
                principalTable: "roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioEvento_roles_TipoUsuariosId",
                table: "UsuarioEvento");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.CreateTable(
                name: "TipoUsuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tipo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoUsuarios", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TipoUsuarios",
                columns: new[] { "Id", "Tipo" },
                values: new object[,]
                {
                    { 1, "Administrador" },
                    { 2, "Convidado" },
                    { 3, "Destinatário" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioEvento_TipoUsuarios_TipoUsuariosId",
                table: "UsuarioEvento",
                column: "TipoUsuariosId",
                principalTable: "TipoUsuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
