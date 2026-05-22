using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OPERACION_OMM.Models;

public partial class DBApiContext : DbContext
{
    public DBApiContext()
    {
    }

    public DBApiContext(DbContextOptions<DBApiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cuentum> Cuenta { get; set; }

    public virtual DbSet<Monedum> Moneda { get; set; }

    public virtual DbSet<Movimiento> Movimientos { get; set; }

    public virtual DbSet<TipoCambio> TipoCambios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cuentum>(entity =>
        {
            entity.HasKey(e => e.NroCuenta).HasName("PK__cuenta__639022113279B57C");

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

            entity.HasOne(d => d.oMoneda).WithMany(p => p.Cuenta)
                .HasForeignKey(d => d.Moneda)
                .HasConstraintName("FK_CUENTA_MONEDA");
        });

        modelBuilder.Entity<Monedum>(entity =>
        {
            entity.HasKey(e => e.Codigo).HasName("PK__moneda__40F9A20762686DAD");

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
            entity.HasKey(e => e.Fecha).HasName("PK__movimien__E11413232F54FB48");

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

            entity.HasOne(d => d.oCuenta).WithMany(p => p.Movimientos)
                .HasForeignKey(d => d.NroCuenta)
                .HasConstraintName("FK_MOVIMEINTO_CUENTA");
        });

        modelBuilder.Entity<TipoCambio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tipo_cam__3213E83FEE1A280E");

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

            entity.HasOne(d => d.oMonedaDestino).WithMany(p => p.TipoCambioMonedaDestinoNavigations)
                .HasForeignKey(d => d.MonedaDestino)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TIPO_CAMBIO_MONEDA_DESTINO");

            entity.HasOne(d => d.oMonedaOrigen).WithMany(p => p.TipoCambioMonedaOrigenNavigations)
                .HasForeignKey(d => d.MonedaOrigen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TIPO_CAMBIO_MONEDA_ORIGEN");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
