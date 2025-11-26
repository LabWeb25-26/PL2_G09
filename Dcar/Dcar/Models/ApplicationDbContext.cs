using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Dcar.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Administrador> Administradors { get; set; }

    public virtual DbSet<Agendum> Agenda { get; set; }

    public virtual DbSet<Anuncio> Anuncios { get; set; }

    public virtual DbSet<Carro> Carros { get; set; }

    public virtual DbSet<Combustivel> Combustivels { get; set; }

    public virtual DbSet<Compra> Compras { get; set; }

    public virtual DbSet<Comprador> Compradors { get; set; }

    public virtual DbSet<Denuncium> Denuncia { get; set; }

    public virtual DbSet<Marca> Marcas { get; set; }

    public virtual DbSet<Modelo> Modelos { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Vendedor> Vendedors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>(entity =>
        {
            entity.HasKey(e => e.IdAdmin).HasName("PK__Administ__89472E95AAE53A56");

            entity.ToTable("Administrador");

            entity.Property(e => e.IdAdmin).HasColumnName("id_admin");
            entity.Property(e => e.Nome)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nome");
            entity.Property(e => e.UserId).HasMaxLength(450);
        });

        modelBuilder.Entity<Agendum>(entity =>
        {
            entity.HasKey(e => e.IdAgenda).HasName("PK__Agenda__178E6FBB0F73CC22");

            entity.Property(e => e.IdAgenda).HasColumnName("id_agenda");
            entity.Property(e => e.DataAgenda).HasColumnName("data_agenda");
            entity.Property(e => e.DataVisita)
                .HasColumnType("datetime")
                .HasColumnName("data_visita");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("pendente")
                .HasColumnName("estado");
            entity.Property(e => e.IdAnuncio).HasColumnName("id_anuncio");
            entity.Property(e => e.IdComprador).HasColumnName("id_comprador");

            entity.HasOne(d => d.IdAnuncioNavigation).WithMany(p => p.Agenda)
                .HasForeignKey(d => d.IdAnuncio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Agenda__id_anunc__02FC7413");

            entity.HasOne(d => d.IdCompradorNavigation).WithMany(p => p.Agenda)
                .HasForeignKey(d => d.IdComprador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Agenda__id_compr__02084FDA");
        });

        modelBuilder.Entity<Anuncio>(entity =>
        {
            entity.HasKey(e => e.IdAnuncio).HasName("PK__Anuncio__4AF1EDBB595BBCE8");

            entity.ToTable("Anuncio");

            entity.HasIndex(e => e.IdCarro, "UQ_Anuncio_Carro").IsUnique();

            entity.Property(e => e.IdAnuncio).HasColumnName("id_anuncio");
            entity.Property(e => e.DataFim).HasColumnName("data_fim");
            entity.Property(e => e.DataInicio).HasColumnName("data_inicio");
            entity.Property(e => e.Descricao)
                .HasColumnType("text")
                .HasColumnName("descricao");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.Fotos)
                .HasColumnType("text")
                .HasColumnName("fotos");
            entity.Property(e => e.IdCarro).HasColumnName("id_carro");
            entity.Property(e => e.IdVendedor).HasColumnName("id_vendedor");
            entity.Property(e => e.Localizacao)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("localizacao");
            entity.Property(e => e.Preco)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("preco");
            entity.Property(e => e.Titulo)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("titulo");

            entity.HasOne(d => d.IdCarroNavigation).WithOne(p => p.Anuncio)
                .HasForeignKey<Anuncio>(d => d.IdCarro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Anuncio__id_carr__787EE5A0");

            entity.HasOne(d => d.IdVendedorNavigation).WithMany(p => p.Anuncios)
                .HasForeignKey(d => d.IdVendedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Anuncio__id_vend__778AC167");
        });

        modelBuilder.Entity<Carro>(entity =>
        {
            entity.HasKey(e => e.IdCarro).HasName("PK__Carro__D3C318A18E59BD0B");

            entity.ToTable("Carro");

            entity.HasIndex(e => e.Matricula, "UQ__Carro__30962D151703B4D1").IsUnique();

            entity.HasIndex(e => e.Vin, "UQ__Carro__C5DF234C60A47882").IsUnique();

            entity.Property(e => e.IdCarro).HasColumnName("id_carro");
            entity.Property(e => e.Ano).HasColumnName("ano");
            entity.Property(e => e.Caixa)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("caixa");
            entity.Property(e => e.IdCombustivel).HasColumnName("id_combustivel");
            entity.Property(e => e.IdModelo).HasColumnName("id_modelo");
            entity.Property(e => e.Matricula)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("matricula");
            entity.Property(e => e.Quilometragem).HasColumnName("quilometragem");
            entity.Property(e => e.Vin)
                .HasMaxLength(17)
                .IsUnicode(false)
                .HasColumnName("VIN");

            entity.HasOne(d => d.IdCombustivelNavigation).WithMany(p => p.Carros)
                .HasForeignKey(d => d.IdCombustivel)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Carro__id_combus__693CA210");

            entity.HasOne(d => d.IdModeloNavigation).WithMany(p => p.Carros)
                .HasForeignKey(d => d.IdModelo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Carro__id_modelo__68487DD7");
        });

        modelBuilder.Entity<Combustivel>(entity =>
        {
            entity.HasKey(e => e.IdCombustivel).HasName("PK__Combusti__59352B78F17ED9EF");

            entity.ToTable("Combustivel");

            entity.HasIndex(e => e.Tipo, "UQ__Combusti__E7F95649E2B6424A").IsUnique();

            entity.Property(e => e.IdCombustivel).HasColumnName("id_combustivel");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.IdCompra).HasName("PK__Compra__C4BAA604E12282D9");

            entity.ToTable("Compra");

            entity.HasIndex(e => e.IdAnuncio, "UQ_Compra_Anuncio").IsUnique();

            entity.Property(e => e.IdCompra).HasColumnName("id_compra");
            entity.Property(e => e.Data)
                .HasColumnType("datetime")
                .HasColumnName("data");
            entity.Property(e => e.EstadoPagamento)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estado_pagamento");
            entity.Property(e => e.IdAnuncio).HasColumnName("id_anuncio");
            entity.Property(e => e.IdComprador).HasColumnName("id_comprador");
            entity.Property(e => e.Preco)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("preco");

            entity.HasOne(d => d.IdAnuncioNavigation).WithOne(p => p.Compra)
                .HasForeignKey<Compra>(d => d.IdAnuncio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Compra__id_anunc__08B54D69");

            entity.HasOne(d => d.IdCompradorNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.IdComprador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Compra__id_compr__07C12930");
        });

        modelBuilder.Entity<Comprador>(entity =>
        {
            entity.HasKey(e => e.IdComprador).HasName("PK__Comprado__BE02FD9B7A2D6A54");

            entity.ToTable("Comprador");

            entity.Property(e => e.IdComprador).HasColumnName("id_comprador");
            entity.Property(e => e.Contactos)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("contactos");
            entity.Property(e => e.Morada)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("morada");
            entity.Property(e => e.Nome)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nome");
            entity.Property(e => e.UserId).HasMaxLength(450);
        });

        modelBuilder.Entity<Denuncium>(entity =>
        {
            entity.HasKey(e => e.IdDenuncia).HasName("PK__Denuncia__2BD955A41A1F5F38");

            entity.Property(e => e.IdDenuncia).HasColumnName("id_denuncia");
            entity.Property(e => e.DataAbertura)
                .HasColumnType("datetime")
                .HasColumnName("data_abertura");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Aberta")
                .HasColumnName("estado");
            entity.Property(e => e.IdAlvoAnuncio).HasColumnName("id_alvo_anuncio");
            entity.Property(e => e.IdDenuncianteUser)
                .HasMaxLength(450)
                .HasColumnName("id_denunciante_user");
            entity.Property(e => e.Motivo)
                .HasColumnType("text")
                .HasColumnName("motivo");

            entity.HasOne(d => d.IdAlvoAnuncioNavigation).WithMany(p => p.Denuncia)
                .HasForeignKey(d => d.IdAlvoAnuncio)
                .HasConstraintName("FK__Denuncia__id_alv__0E6E26BF");
        });

        modelBuilder.Entity<Marca>(entity =>
        {
            entity.HasKey(e => e.IdMarca).HasName("PK__Marca__7E43E99E9E59784A");

            entity.ToTable("Marca");

            entity.HasIndex(e => e.NomeMarca, "UQ__Marca__93217C604DFFABE0").IsUnique();

            entity.Property(e => e.IdMarca).HasColumnName("id_marca");
            entity.Property(e => e.NomeMarca)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nome_marca");
        });

        modelBuilder.Entity<Modelo>(entity =>
        {
            entity.HasKey(e => e.IdModelo).HasName("PK__Modelo__B3BFCFF1367160B2");

            entity.ToTable("Modelo");

            entity.HasIndex(e => new { e.NomeModelo, e.IdMarca }, "UQ__Modelo__BC48DA93621EF7FF").IsUnique();

            entity.Property(e => e.IdModelo).HasColumnName("id_modelo");
            entity.Property(e => e.IdMarca).HasColumnName("id_marca");
            entity.Property(e => e.NomeModelo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nome_modelo");

            entity.HasOne(d => d.IdMarcaNavigation).WithMany(p => p.Modelos)
                .HasForeignKey(d => d.IdMarca)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Modelo__id_marca__6383C8BA");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.IdReserva).HasName("PK__Reserva__423CBE5DEAA43B50");

            entity.ToTable("Reserva");

            entity.Property(e => e.IdReserva).HasColumnName("id_reserva");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.IdAnuncio).HasColumnName("id_anuncio");
            entity.Property(e => e.IdComprador).HasColumnName("id_comprador");
            entity.Property(e => e.PrazoExpiracao).HasColumnName("prazo_expiracao");

            entity.HasOne(d => d.IdAnuncioNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdAnuncio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reserva__id_anun__7C4F7684");

            entity.HasOne(d => d.IdCompradorNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdComprador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reserva__id_comp__7D439ABD");
        });

        modelBuilder.Entity<Vendedor>(entity =>
        {
            entity.HasKey(e => e.IdVendedor).HasName("PK__Vendedor__00930308EDA20D20");

            entity.ToTable("Vendedor");

            entity.Property(e => e.IdVendedor).HasColumnName("id_vendedor");
            entity.Property(e => e.Contactos)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("contactos");
            entity.Property(e => e.EstadoAprovacao)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("pendente")
                .HasColumnName("estado_aprovacao");
            entity.Property(e => e.Morada)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("morada");
            entity.Property(e => e.Nif)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("NIF");
            entity.Property(e => e.Nome)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nome");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tipo");
            entity.Property(e => e.UserId).HasMaxLength(450);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
