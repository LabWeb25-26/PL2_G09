using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DCarMarketplace.Data.Migrations
{
    /// <inheritdoc />
    public partial class atualizacaofavoritos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendas_Anuncios_AnuncioId",
                table: "Agendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Compras_Anuncios_AnuncioId",
                table: "Compras");

            migrationBuilder.DropForeignKey(
                name: "FK_FiltrosFavoritos_Compradores_CompradorId",
                table: "FiltrosFavoritos");

            migrationBuilder.DropForeignKey(
                name: "FK_MarcasFavoritas_Compradores_CompradorId",
                table: "MarcasFavoritas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Anuncios_AnuncioId",
                table: "Reservas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MarcasFavoritas",
                table: "MarcasFavoritas");

            migrationBuilder.RenameColumn(
                name: "NomeFiltro",
                table: "FiltrosFavoritos",
                newName: "UrlQuery");

            migrationBuilder.RenameColumn(
                name: "Criterios",
                table: "FiltrosFavoritos",
                newName: "Nome");

            migrationBuilder.AddColumn<int>(
                name: "AnuncioId1",
                table: "Reservas",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompradorId",
                table: "MarcasFavoritas",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UtilizadorId",
                table: "MarcasFavoritas",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "CompradorId",
                table: "FiltrosFavoritos",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCriacao",
                table: "FiltrosFavoritos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UtilizadorId",
                table: "FiltrosFavoritos",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AnuncioId1",
                table: "Compras",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AnuncioId1",
                table: "Agendas",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarcasFavoritas",
                table: "MarcasFavoritas",
                columns: new[] { "UtilizadorId", "MarcaId" });

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_AnuncioId1",
                table: "Reservas",
                column: "AnuncioId1");

            migrationBuilder.CreateIndex(
                name: "IX_MarcasFavoritas_CompradorId",
                table: "MarcasFavoritas",
                column: "CompradorId");

            migrationBuilder.CreateIndex(
                name: "IX_FiltrosFavoritos_UtilizadorId",
                table: "FiltrosFavoritos",
                column: "UtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_AnuncioId1",
                table: "Compras",
                column: "AnuncioId1",
                unique: true,
                filter: "[AnuncioId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Agendas_AnuncioId1",
                table: "Agendas",
                column: "AnuncioId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendas_Anuncios_AnuncioId",
                table: "Agendas",
                column: "AnuncioId",
                principalTable: "Anuncios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendas_Anuncios_AnuncioId1",
                table: "Agendas",
                column: "AnuncioId1",
                principalTable: "Anuncios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Compras_Anuncios_AnuncioId",
                table: "Compras",
                column: "AnuncioId",
                principalTable: "Anuncios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Compras_Anuncios_AnuncioId1",
                table: "Compras",
                column: "AnuncioId1",
                principalTable: "Anuncios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FiltrosFavoritos_AspNetUsers_UtilizadorId",
                table: "FiltrosFavoritos",
                column: "UtilizadorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FiltrosFavoritos_Compradores_CompradorId",
                table: "FiltrosFavoritos",
                column: "CompradorId",
                principalTable: "Compradores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarcasFavoritas_AspNetUsers_UtilizadorId",
                table: "MarcasFavoritas",
                column: "UtilizadorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarcasFavoritas_Compradores_CompradorId",
                table: "MarcasFavoritas",
                column: "CompradorId",
                principalTable: "Compradores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Anuncios_AnuncioId",
                table: "Reservas",
                column: "AnuncioId",
                principalTable: "Anuncios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Anuncios_AnuncioId1",
                table: "Reservas",
                column: "AnuncioId1",
                principalTable: "Anuncios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendas_Anuncios_AnuncioId",
                table: "Agendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendas_Anuncios_AnuncioId1",
                table: "Agendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Compras_Anuncios_AnuncioId",
                table: "Compras");

            migrationBuilder.DropForeignKey(
                name: "FK_Compras_Anuncios_AnuncioId1",
                table: "Compras");

            migrationBuilder.DropForeignKey(
                name: "FK_FiltrosFavoritos_AspNetUsers_UtilizadorId",
                table: "FiltrosFavoritos");

            migrationBuilder.DropForeignKey(
                name: "FK_FiltrosFavoritos_Compradores_CompradorId",
                table: "FiltrosFavoritos");

            migrationBuilder.DropForeignKey(
                name: "FK_MarcasFavoritas_AspNetUsers_UtilizadorId",
                table: "MarcasFavoritas");

            migrationBuilder.DropForeignKey(
                name: "FK_MarcasFavoritas_Compradores_CompradorId",
                table: "MarcasFavoritas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Anuncios_AnuncioId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Anuncios_AnuncioId1",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_AnuncioId1",
                table: "Reservas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MarcasFavoritas",
                table: "MarcasFavoritas");

            migrationBuilder.DropIndex(
                name: "IX_MarcasFavoritas_CompradorId",
                table: "MarcasFavoritas");

            migrationBuilder.DropIndex(
                name: "IX_FiltrosFavoritos_UtilizadorId",
                table: "FiltrosFavoritos");

            migrationBuilder.DropIndex(
                name: "IX_Compras_AnuncioId1",
                table: "Compras");

            migrationBuilder.DropIndex(
                name: "IX_Agendas_AnuncioId1",
                table: "Agendas");

            migrationBuilder.DropColumn(
                name: "AnuncioId1",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "UtilizadorId",
                table: "MarcasFavoritas");

            migrationBuilder.DropColumn(
                name: "DataCriacao",
                table: "FiltrosFavoritos");

            migrationBuilder.DropColumn(
                name: "UtilizadorId",
                table: "FiltrosFavoritos");

            migrationBuilder.DropColumn(
                name: "AnuncioId1",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "AnuncioId1",
                table: "Agendas");

            migrationBuilder.RenameColumn(
                name: "UrlQuery",
                table: "FiltrosFavoritos",
                newName: "NomeFiltro");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "FiltrosFavoritos",
                newName: "Criterios");

            migrationBuilder.AlterColumn<string>(
                name: "CompradorId",
                table: "MarcasFavoritas",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompradorId",
                table: "FiltrosFavoritos",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarcasFavoritas",
                table: "MarcasFavoritas",
                columns: new[] { "CompradorId", "MarcaId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Agendas_Anuncios_AnuncioId",
                table: "Agendas",
                column: "AnuncioId",
                principalTable: "Anuncios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Compras_Anuncios_AnuncioId",
                table: "Compras",
                column: "AnuncioId",
                principalTable: "Anuncios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FiltrosFavoritos_Compradores_CompradorId",
                table: "FiltrosFavoritos",
                column: "CompradorId",
                principalTable: "Compradores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarcasFavoritas_Compradores_CompradorId",
                table: "MarcasFavoritas",
                column: "CompradorId",
                principalTable: "Compradores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Anuncios_AnuncioId",
                table: "Reservas",
                column: "AnuncioId",
                principalTable: "Anuncios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
