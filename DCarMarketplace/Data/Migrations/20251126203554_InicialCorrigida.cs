using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DCarMarketplace.Data.Migrations
{
    /// <inheritdoc />
    public partial class InicialCorrigida : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataRegisto",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "EstadoConta",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "AspNetUsers",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Administradores",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administradores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Administradores_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Combustiveis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Combustiveis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Compradores",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Morada = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Contactos = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compradores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Compradores_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Marcas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marcas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vendedores",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Morada = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Contactos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NIF = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstadoAprovacao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendedores", x => x.Id);
                    table.CheckConstraint("CK_Vendedor_EstadoAprovacao", "EstadoAprovacao IN ('pendente', 'aprovado', 'rejeitado')");
                    table.ForeignKey(
                        name: "FK_Vendedores_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FiltrosFavoritos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompradorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NomeFiltro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Criterios = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiltrosFavoritos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FiltrosFavoritos_Compradores_CompradorId",
                        column: x => x.CompradorId,
                        principalTable: "Compradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarcasFavoritas",
                columns: table => new
                {
                    CompradorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MarcaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarcasFavoritas", x => new { x.CompradorId, x.MarcaId });
                    table.ForeignKey(
                        name: "FK_MarcasFavoritas_Compradores_CompradorId",
                        column: x => x.CompradorId,
                        principalTable: "Compradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarcasFavoritas_Marcas_MarcaId",
                        column: x => x.MarcaId,
                        principalTable: "Marcas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Modelos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarcaId = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modelos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modelos_Marcas_MarcaId",
                        column: x => x.MarcaId,
                        principalTable: "Marcas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Carros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VIN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Matricula = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    Quilometragem = table.Column<int>(type: "int", nullable: false),
                    Caixa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModeloId = table.Column<int>(type: "int", nullable: false),
                    CombustivelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carros_Combustiveis_CombustivelId",
                        column: x => x.CombustivelId,
                        principalTable: "Combustiveis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Carros_Modelos_ModeloId",
                        column: x => x.ModeloId,
                        principalTable: "Modelos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Anuncios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendedorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CarroId = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Localizacao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fotos = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anuncios", x => x.Id);
                    table.CheckConstraint("CK_Anuncio_Estado", "Estado IN ('ativo', 'reservado', 'vendido', 'pausado')");
                    table.CheckConstraint("CK_Anuncio_Preco", "Preco >= 0");
                    table.ForeignKey(
                        name: "FK_Anuncios_Carros_CarroId",
                        column: x => x.CarroId,
                        principalTable: "Carros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Anuncios_Vendedores_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "Vendedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Agendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnuncioId = table.Column<int>(type: "int", nullable: false),
                    CompradorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DataAgenda = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataVisita = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agendas", x => x.Id);
                    table.CheckConstraint("CK_Agenda_Estado", "Estado IN ('pendente', 'realizada', 'cancelada')");
                    table.ForeignKey(
                        name: "FK_Agendas_Anuncios_AnuncioId",
                        column: x => x.AnuncioId,
                        principalTable: "Anuncios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agendas_Compradores_CompradorId",
                        column: x => x.CompradorId,
                        principalTable: "Compradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnuncioId = table.Column<int>(type: "int", nullable: false),
                    CompradorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstadoPagamento = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compras", x => x.Id);
                    table.CheckConstraint("CK_Compra_EstadoPagamento", "EstadoPagamento IN ('pendente', 'pago', 'cancelado')");
                    table.ForeignKey(
                        name: "FK_Compras_Anuncios_AnuncioId",
                        column: x => x.AnuncioId,
                        principalTable: "Anuncios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Compras_Compradores_CompradorId",
                        column: x => x.CompradorId,
                        principalTable: "Compradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistoricoAcoesAdmin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdminId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AlvoUtilizadorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AlvoAnuncioId = table.Column<int>(type: "int", nullable: true),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoAcao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoAcoesAdmin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricoAcoesAdmin_Administradores_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Administradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistoricoAcoesAdmin_Anuncios_AlvoAnuncioId",
                        column: x => x.AlvoAnuncioId,
                        principalTable: "Anuncios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HistoricoAcoesAdmin_AspNetUsers_AlvoUtilizadorId",
                        column: x => x.AlvoUtilizadorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnuncioId = table.Column<int>(type: "int", nullable: false),
                    CompradorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrazoExpiracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.Id);
                    table.CheckConstraint("CK_Reserva_Estado", "Estado IN ('ativa', 'expirada', 'cancelada')");
                    table.ForeignKey(
                        name: "FK_Reservas_Anuncios_AnuncioId",
                        column: x => x.AnuncioId,
                        principalTable: "Anuncios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservas_Compradores_CompradorId",
                        column: x => x.CompradorId,
                        principalTable: "Compradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Utilizador_EstadoConta",
                table: "AspNetUsers",
                sql: "EstadoConta IN ('ativo', 'bloqueado')");

            migrationBuilder.CreateIndex(
                name: "IX_Agendas_AnuncioId",
                table: "Agendas",
                column: "AnuncioId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendas_CompradorId",
                table: "Agendas",
                column: "CompradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Anuncios_CarroId",
                table: "Anuncios",
                column: "CarroId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Anuncios_VendedorId",
                table: "Anuncios",
                column: "VendedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Carros_CombustivelId",
                table: "Carros",
                column: "CombustivelId");

            migrationBuilder.CreateIndex(
                name: "IX_Carros_ModeloId",
                table: "Carros",
                column: "ModeloId");

            migrationBuilder.CreateIndex(
                name: "IX_Combustiveis_Tipo",
                table: "Combustiveis",
                column: "Tipo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Compras_AnuncioId",
                table: "Compras",
                column: "AnuncioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Compras_CompradorId",
                table: "Compras",
                column: "CompradorId");

            migrationBuilder.CreateIndex(
                name: "IX_FiltrosFavoritos_CompradorId",
                table: "FiltrosFavoritos",
                column: "CompradorId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoAcoesAdmin_AdminId",
                table: "HistoricoAcoesAdmin",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoAcoesAdmin_AlvoAnuncioId",
                table: "HistoricoAcoesAdmin",
                column: "AlvoAnuncioId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoAcoesAdmin_AlvoUtilizadorId",
                table: "HistoricoAcoesAdmin",
                column: "AlvoUtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Marcas_Nome",
                table: "Marcas",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarcasFavoritas_MarcaId",
                table: "MarcasFavoritas",
                column: "MarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_Modelos_MarcaId",
                table: "Modelos",
                column: "MarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_AnuncioId",
                table: "Reservas",
                column: "AnuncioId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_CompradorId",
                table: "Reservas",
                column: "CompradorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agendas");

            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "FiltrosFavoritos");

            migrationBuilder.DropTable(
                name: "HistoricoAcoesAdmin");

            migrationBuilder.DropTable(
                name: "MarcasFavoritas");

            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "Administradores");

            migrationBuilder.DropTable(
                name: "Anuncios");

            migrationBuilder.DropTable(
                name: "Compradores");

            migrationBuilder.DropTable(
                name: "Carros");

            migrationBuilder.DropTable(
                name: "Vendedores");

            migrationBuilder.DropTable(
                name: "Combustiveis");

            migrationBuilder.DropTable(
                name: "Modelos");

            migrationBuilder.DropTable(
                name: "Marcas");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Utilizador_EstadoConta",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DataRegisto",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EstadoConta",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "AspNetUsers");
        }
    }
}
