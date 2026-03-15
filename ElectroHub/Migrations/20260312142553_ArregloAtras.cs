using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroHub.Migrations
{
    /// <inheritdoc />
    public partial class ArregloAtras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoriaId",
                table: "DetallesVentas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVentas_CategoriaId",
                table: "DetallesVentas",
                column: "CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesVentas_Categorias_CategoriaId",
                table: "DetallesVentas",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "CategoriaId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
