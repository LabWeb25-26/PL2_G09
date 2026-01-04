using DCarMarketplace.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DCarMarketplace.Data
{
    public class ApplicationDbContext : IdentityDbContext<Utilizador>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // --- 1. Tabelas de Utilizadores e Perfis ---
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Vendedor> Vendedores { get; set; }
        public DbSet<Comprador> Compradores { get; set; }
                                                                                                                                                                                          
        // --- 2. Tabelas de Domínio (Carros) ---
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Modelo> Modelos { get; set; }
        public DbSet<Combustivel> Combustiveis { get; set; }
        public DbSet<Carro> Carros { get; set; }

        // --- 3. Tabelas de Negócio (Marketplace) ---
        public DbSet<Anuncio> Anuncios { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Agenda> Agendas { get; set; }
        public DbSet<Compra> Compras { get; set; }

        // --- 4. Tabelas Auxiliares e Auditoria ---
        public DbSet<FiltroFavorito> FiltrosFavoritos { get; set; }
        public DbSet<MarcaFavorita> MarcasFavoritas { get; set; }
        public DbSet<HistoricoAcaoAdmin> HistoricoAcoesAdmin { get; set; }

        // Tabela de Fotos
        public DbSet<Foto> Fotos { get; set; }

        // Tabela de Favoritos (Corações)
        public DbSet<AnuncioFavorito> AnunciosFavoritos { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // =========================================================================
            // A. CONFIGURAÇÃO DE RELAÇÕES 1-PARA-1 (PERFIS)
            // =========================================================================

            builder.Entity<Utilizador>()
                .HasOne(u => u.Vendedor)
                .WithOne(v => v.Utilizador)
                .HasForeignKey<Vendedor>(v => v.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Utilizador>()
                .HasOne(u => u.Comprador)
                .WithOne(c => c.Utilizador)
                .HasForeignKey<Comprador>(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Utilizador>()
                .HasOne(u => u.Administrador)
                .WithOne(a => a.Utilizador)
                .HasForeignKey<Administrador>(a => a.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================================================================
            // B. CORREÇÃO DE CICLOS DE CASCATA (ERRO SQL 1785)
            // =========================================================================

            // 1. Agendas
            builder.Entity<Agenda>()
                .HasOne(a => a.Comprador)
                .WithMany(c => c.VisitasAgendadas)
                .HasForeignKey(a => a.CompradorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Agenda>()
                .HasOne(a => a.Anuncio)
                .WithMany()
                .HasForeignKey(a => a.AnuncioId)
                .OnDelete(DeleteBehavior.Restrict); // Adicionado para evitar ciclo com Anuncio

            // 2. Reservas
            builder.Entity<Reserva>()
                .HasOne(r => r.Comprador)
                .WithMany(c => c.Reservas)
                .HasForeignKey(r => r.CompradorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Reserva>()
                .HasOne(r => r.Anuncio)
                .WithMany()
                .HasForeignKey(r => r.AnuncioId)
                .OnDelete(DeleteBehavior.Restrict); // Adicionado para segurança

            // 3. Compras
            builder.Entity<Compra>()
                .HasOne(c => c.Comprador)
                .WithMany(c => c.Compras)
                .HasForeignKey(c => c.CompradorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Compra>()
                .HasOne(c => c.Anuncio)
                .WithOne()
                .HasForeignKey<Compra>(c => c.AnuncioId)
                .OnDelete(DeleteBehavior.Restrict); // Adicionado para segurança

            // 4. Favoritos
            builder.Entity<AnuncioFavorito>()
                .HasOne(f => f.Utilizador)
                .WithMany()
                .HasForeignKey(f => f.UtilizadorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AnuncioFavorito>()
                .HasOne(f => f.Anuncio)
                .WithMany()
                .HasForeignKey(f => f.AnuncioId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================================================
            // C. CHAVES COMPOSTAS E ÍNDICES ÚNICOS
            // =========================================================================

            // *** AQUI ESTAVA O ERRO: Mudei CompradorId para UtilizadorId ***
            builder.Entity<MarcaFavorita>()
                .HasKey(mf => new { mf.UtilizadorId, mf.MarcaId });

            builder.Entity<Anuncio>()
                .HasIndex(a => a.CarroId)
                .IsUnique();

            builder.Entity<Compra>()
                .HasIndex(c => c.AnuncioId)
                .IsUnique();

            builder.Entity<Marca>().HasIndex(m => m.Nome).IsUnique();
            builder.Entity<Combustivel>().HasIndex(c => c.Tipo).IsUnique();

            // =========================================================================
            // D. REGRAS DE INTEGRIDADE (CHECK CONSTRAINTS)
            // =========================================================================

            builder.Entity<Utilizador>()
                .ToTable(t => t.HasCheckConstraint("CK_Utilizador_EstadoConta",
                    "EstadoConta IN ('ativo', 'bloqueado')"));

            builder.Entity<Vendedor>()
                .ToTable(t => t.HasCheckConstraint("CK_Vendedor_EstadoAprovacao",
                    "EstadoAprovacao IN ('pendente', 'aprovado', 'rejeitado')"));

            builder.Entity<Anuncio>()
                .ToTable(t => t.HasCheckConstraint("CK_Anuncio_Estado",
                    "Estado IN ('ativo', 'reservado', 'vendido', 'pausado')"));

            builder.Entity<Anuncio>()
                .ToTable(t => t.HasCheckConstraint("CK_Anuncio_Preco", "Preco >= 0"));

            builder.Entity<Reserva>()
                .ToTable(t => t.HasCheckConstraint("CK_Reserva_Estado",
                    "Estado IN ('ativa', 'expirada', 'cancelada')"));

            builder.Entity<Agenda>()
                .ToTable(t => t.HasCheckConstraint("CK_Agenda_Estado",
                    "Estado IN ('pendente', 'realizada', 'cancelada')"));

            builder.Entity<Compra>()
                .ToTable(t => t.HasCheckConstraint("CK_Compra_EstadoPagamento",
                    "EstadoPagamento IN ('pendente', 'pago', 'cancelado')"));

            // =========================================================================
            // E. NOMES DAS TABELAS
            // =========================================================================
            builder.Entity<Utilizador>().ToTable("AspNetUsers");
            builder.Entity<Vendedor>().ToTable("Vendedores");
            builder.Entity<Comprador>().ToTable("Compradores");
            builder.Entity<Administrador>().ToTable("Administradores");
        }
    }
}