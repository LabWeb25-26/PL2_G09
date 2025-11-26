using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DCARMarketplace.Models;

namespace DCARMarketplace.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Modelo> Modelos { get; set; }
        public DbSet<Combustivel> Combustiveis { get; set; }
        public DbSet<Carro> Carros { get; set; }

        // Utilizadores
        public DbSet<Utilizador> UtilizadoresGerais { get; set; }
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Vendedor> Vendedores { get; set; }
        public DbSet<Comprador> Compradores { get; set; }

        public DbSet<Anuncio> Anuncios { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Agenda> Agendas { get; set; }
        public DbSet<Compra> Compras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Configuração TPT (Table Per Type) para Utilizadores
            modelBuilder.Entity<Utilizador>().UseTptMappingStrategy();

            // 2. Garantir unicidade
            modelBuilder.Entity<Carro>().HasIndex(c => c.Matricula).IsUnique();
            modelBuilder.Entity<Marca>().HasIndex(m => m.Nome).IsUnique();
            modelBuilder.Entity<Combustivel>().HasIndex(c => c.Tipo).IsUnique();

            // 3. Configurar preços
            modelBuilder.Entity<Anuncio>().Property(p => p.Preco).HasColumnType("decimal(10, 2)");
            modelBuilder.Entity<Compra>().Property(p => p.Preco).HasColumnType("decimal(10, 2)");

            // 4. RESOLUÇÃO DO ERRO DE CASCATA (Ciclos)
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Comprador)
                .WithMany()
                .HasForeignKey(r => r.CompradorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Agenda>()
                .HasOne(a => a.Comprador)
                .WithMany()
                .HasForeignKey(a => a.CompradorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Compra>()
                .HasOne(c => c.Comprador)
                .WithMany()
                .HasForeignKey(c => c.CompradorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}