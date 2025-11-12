using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;

namespace OMPS.DBModels.Material;

public partial class MaterialDbCtx : DbContext
{
    public MaterialDbCtx()
    {
    }

    public MaterialDbCtx(DbContextOptions<MaterialDbCtx> options)
        : base(options)
    {
    }

    public virtual DbSet<VwAcrylicList> VwAcrylicLists { get; set; }

    public virtual DbSet<VwChaseDoorList> VwChaseDoorLists { get; set; }

    public virtual DbSet<VwCurvePnlLamList> VwCurvePnlLamLists { get; set; }

    public virtual DbSet<VwFabricList> VwFabricLists { get; set; }

    public virtual DbSet<VwHcdoorLamList> VwHcdoorLamLists { get; set; }

    public virtual DbSet<VwLamPnlList> VwLamPnlLists { get; set; }

    public virtual DbSet<VwTflpnlList> VwTflpnlLists { get; set; }

    public virtual DbSet<VwWsedgeList> VwWsedgeLists { get; set; }

    public virtual DbSet<VwWslamList> VwWslamLists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["NewOldCrm"].ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VwAcrylicList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwAcrylic_List", "dbo");

            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.Item).HasMaxLength(50);
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.ProductCategory)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.ProductSubCategory)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<VwChaseDoorList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwChaseDoor_List", "dbo");

            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.Item).HasMaxLength(50);
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.ProductCategory)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.ProductSubCategory)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<VwCurvePnlLamList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwCurvePnlLam_List", "dbo");

            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.Item).HasMaxLength(50);
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.ProductCategory)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.ProductSubCategory)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<VwFabricList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwFabric_List", "dbo");

            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.Item).HasMaxLength(50);
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.ProductCategory)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.ProductSubCategory)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<VwHcdoorLamList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwHCdoorLam_List", "dbo");

            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.Item).HasMaxLength(50);
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.ProductCategory)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.ProductSubCategory)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<VwLamPnlList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwLamPnl_List", "dbo");

            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.Item).HasMaxLength(50);
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.ProductCategory)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.ProductSubCategory)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<VwTflpnlList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwTFLPnl_List", "dbo");

            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.Item).HasMaxLength(50);
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.ProductCategory)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.ProductSubCategory)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<VwWsedgeList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwWSEdge_List", "dbo");

            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.Item).HasMaxLength(50);
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.ProductCategory)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.ProductSubCategory)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<VwWslamList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwWSLam_List", "dbo");

            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.Item).HasMaxLength(50);
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.ProductCategory)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.ProductSubCategory)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
