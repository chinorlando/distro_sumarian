using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OPERACION_OMM_NEW.Models;

public partial class DBApiContext : DbContext
{
    public DBApiContext()
    {
    }

    public DBApiContext(DbContextOptions<DBApiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cuenta> Cuenta { get; set; }

    public virtual DbSet<Moneda> Moneda { get; set; }

    public virtual DbSet<Movimiento> Movimiento { get; set; }

    public virtual DbSet<TipoCambio> TipoCambio { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cuenta>(entity =>
        {
            entity.HasKey(e => e.NroCuenta).HasName("PK__cuenta__63902211601B4F1A");

            entity.ToTable("cuenta");

            entity.Property(e => e.NroCuenta)
                .HasMaxLength(14)
                .HasColumnName("nro_cuenta");
            entity.Property(e => e.Moneda)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("moneda");
            entity.Property(e => e.Nombre)
                .HasMaxLength(40)
                .HasColumnName("nombre");
            entity.Property(e => e.Saldo)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("saldo");
            entity.Property(e => e.Tipo)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("tipo");

            entity.HasOne(d => d.OMoneda).WithMany(p => p.Cuenta)
                .HasForeignKey(d => d.Moneda)
                .HasConstraintName("FK_CUENTA_MONEDA");
        });

        modelBuilder.Entity<Moneda>(entity =>
        {
            entity.HasKey(e => e.Codigo).HasName("PK__moneda__40F9A20799E7B13A");

            entity.ToTable("moneda");

            entity.Property(e => e.Codigo)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("codigo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.Simbolo)
                .HasMaxLength(5)
                .HasColumnName("simbolo");
        });

        modelBuilder.Entity<Movimiento>(entity =>
        {
            entity.HasKey(e => e.Fecha).HasName("PK__movimien__E11413238A655A78");

            entity.ToTable("movimiento");

            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.Importe)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("importe");
            entity.Property(e => e.NroCuenta)
                .HasMaxLength(14)
                .HasColumnName("nro_cuenta");
            entity.Property(e => e.Tipo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("tipo");

            entity.HasOne(d => d.OCuenta).WithMany(p => p.Movimiento)
                .HasForeignKey(d => d.NroCuenta)
                .HasConstraintName("FK_MOVIMEINTO_CUENTA");
        });

        modelBuilder.Entity<TipoCambio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tipo_cam__3213E83F7A0C4BAA");

            entity.ToTable("tipo_cambio");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.MonedaDestino)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("moneda_destino");
            entity.Property(e => e.MonedaOrigen)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("moneda_origen");
            entity.Property(e => e.Tasa)
                .HasColumnType("decimal(10, 6)")
                .HasColumnName("tasa");

            entity.HasOne(d => d.OMonedaDestino).WithMany(p => p.OTipoCambioMonedaDestino)
                .HasForeignKey(d => d.MonedaDestino)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TIPO_CAMBIO_MONEDA_DESTINO");

            entity.HasOne(d => d.OMonedaOrigen).WithMany(p => p.OTipoCambioMonedaOrigen)
                .HasForeignKey(d => d.MonedaOrigen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TIPO_CAMBIO_MONEDA_ORIGEN");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
