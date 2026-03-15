using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroHub.Migrations
{
    /// <inheritdoc />
    public partial class ArregloGuardado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallesVentas_Categorias_CategoriaId",
                table: "DetallesVentas");

            migrationBuilder.DropIndex(
                name: "IX_DetallesVentas_CategoriaId",
                table: "DetallesVentas");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "DetallesVentas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoriaId",
                table: "DetallesVentas",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVentas_CategoriaId",
                table: "DetallesVentas",
                column: "CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesVentas_Categorias_CategoriaId",
                table: "DetallesVentas",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "CategoriaId");
        }
    }
}
