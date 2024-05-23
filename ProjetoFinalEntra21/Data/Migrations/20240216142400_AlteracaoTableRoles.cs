using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoFinalEntra21.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlteracaoTableRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioEvento_roles_TipoUsuariosId",
                table: "UsuarioEvento");

            migrationBuilder.DropPrimaryKey(
                name: "PK_roles",
                table: "roles");

            migrationBuilder.RenameTable(
                name: "roles",
                newName: "Roles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioEvento_Roles_TipoUsuariosId",
                table: "UsuarioEvento",
                column: "TipoUsuariosId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioEvento_Roles_TipoUsuariosId",
                table: "UsuarioEvento");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "roles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_roles",
                table: "roles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioEvento_roles_TipoUsuariosId",
                table: "UsuarioEvento",
                column: "TipoUsuariosId",
                principalTable: "roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
