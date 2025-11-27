using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DCarMarketplace.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarFavoritos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnunciosFavoritos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtilizadorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AnuncioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnunciosFavoritos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnunciosFavoritos_Anuncios_AnuncioId",
                        column: x => x.AnuncioId,
                        principalTable: "Anuncios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnunciosFavoritos_AspNetUsers_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnunciosFavoritos_AnuncioId",
                table: "AnunciosFavoritos",
                column: "AnuncioId");

            migrationBuilder.CreateIndex(
                name: "IX_AnunciosFavoritos_UtilizadorId",
                table: "AnunciosFavoritos",
                column: "UtilizadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnunciosFavoritos");
        }
    }
}
