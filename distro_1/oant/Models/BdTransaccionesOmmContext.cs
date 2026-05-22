using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OPERACION_OMM.Models;

public partial class BdTransaccionesOmmContext : DbContext
{
    public BdTransaccionesOmmContext()
    {
    }

    public BdTransaccionesOmmContext(DbContextOptions<BdTransaccionesOmmContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cuenta> Cuenta { get; set; }

    public virtual DbSet<Movimiento> Movimiento { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cuenta>(entity =>
        {
            entity.HasKey(e => e.NroCuenta);

            entity.ToTable("CUENTA");

            entity.Property(e => e.NroCuenta)
                .HasMaxLength(14)
                .HasColumnName("NRO_CUENTA");
            entity.Property(e => e.Moneda)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MONEDA");
            entity.Property(e => e.Nombre)
                .HasMaxLength(40)
                .HasColumnName("NOMBRE");
            entity.Property(e => e.Saldo)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("SALDO");
            entity.Property(e => e.Tipo)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TIPO");
        });

        modelBuilder.Entity<Movimiento>(entity =>
        {
            entity.HasKey(e => new { e.NroCuenta, e.Fecha });

            entity.ToTable("MOVIMIENTO");

            entity.Property(e => e.NroCuenta)
                .HasMaxLength(14)
                .HasColumnName("NRO_CUENTA");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA");
            entity.Property(e => e.Importe)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("IMPORTE");
            entity.Property(e => e.Tipo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TIPO");

            entity.HasOne(d => d.OCuenta).WithMany(p => p.Movimiento)
                .HasForeignKey(d => d.NroCuenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MOVIMIENTO_CUENTA");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
