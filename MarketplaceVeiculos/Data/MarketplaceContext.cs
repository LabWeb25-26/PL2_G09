using Microsoft.EntityFrameworkCore;
using MarketplaceVeiculos.Models; // Importa os teus models

namespace MarketplaceVeiculos.Data
{
    public class MarketplaceContext : DbContext
    {
        // Construtor
        public MarketplaceContext(DbContextOptions<MarketplaceContext> options) : base(options)
        {
        }

        // --- Mapeia os teus Models para as tabelas da Base de Dados ---

        // Grupo Veículo
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Combustivel> Combustiveis { get; set; }
        public DbSet<Modelo> Modelos { get; set; }
        public DbSet<Carro> Carros { get; set; }

        // Grupo Utilizadores e Perfis
        public DbSet<Utilizador> Utilizadores { get; set; }
        public DbSet<Vendedor> Vendedores { get; set; }
        public DbSet<Comprador> Compradores { get; set; }
        public DbSet<Administrador> Administradores { get; set; }

        // Grupo Marketplace e Transações
        public DbSet<Anuncio> Anuncios { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Agenda> Agendas { get; set; }
        public DbSet<Compra> Compras { get; set; }


        // --- Configuração das Chaves Primárias Compostas ---
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configura a Chave Primária Composta para Reserva
            modelBuilder.Entity<Reserva>()
                .HasKey(r => new { r.id_anuncio, r.id_comprador });

            // Configura a Chave Primária Composta para Compra
            modelBuilder.Entity<Compra>()
                .HasKey(c => new { c.id_anuncio, c.id_comprador });
        }
    }
}