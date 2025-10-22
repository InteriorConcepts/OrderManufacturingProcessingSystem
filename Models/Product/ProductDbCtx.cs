using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;

namespace OMPS.Models.Product;

public partial class ProductDbCtx : DbContext
{
    public ProductDbCtx()
    {
    }

    public ProductDbCtx(DbContextOptions<ProductDbCtx> options)
        : base(options)
    {
    }

    public virtual DbSet<AIcMfgBom> AIcMfgBoms { get; set; }

    public virtual DbSet<AIcProdBom> AIcProdBoms { get; set; }

    public virtual DbSet<IcItem> IcItems { get; set; }

    public virtual DbSet<IcProductCatalog> IcProductCatalogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["NewOldCrm"].ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AIcMfgBom>(entity =>
        {
            entity.HasKey(e => e.MfgBomId);

            entity.ToTable("aIC_MfgBom", "dbo");

            entity.HasIndex(e => e.ItemNbr, "IX_aIC_MfgBom");

            entity.Property(e => e.MfgBomId)
                .ValueGeneratedNever()
                .HasColumnName("MfgBomID");
            entity.Property(e => e.Assembled).HasMaxLength(50);
            entity.Property(e => e.ChangeDate).HasColumnType("datetime");
            entity.Property(e => e.ChangedById).HasColumnName("ChangedByID");
            entity.Property(e => e.CreationDate).HasColumnType("datetime");
            entity.Property(e => e.Dept).HasMaxLength(8);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.ItemNbr)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Type).HasMaxLength(6);
            entity.Property(e => e.UofM)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.WorkCtr).HasMaxLength(8);

            entity.HasOne(d => d.Product).WithMany(p => p.AIcMfgBoms)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_aIC_MfgBom__IC_ProductCatalog");
        });

        modelBuilder.Entity<AIcProdBom>(entity =>
        {
            entity.ToTable("aIC_ProdBom", "dbo");

            entity.HasIndex(e => e.ProductCode, "IX_aIC_ProdBom");

            entity.Property(e => e.AIcProdBomId)
                .ValueGeneratedNever()
                .HasColumnName("aIC_ProdBomID");
            entity.Property(e => e.Assembled).HasMaxLength(50);
            entity.Property(e => e.ChangeDate).HasColumnType("datetime");
            entity.Property(e => e.ChangedById).HasColumnName("ChangedByID");
            entity.Property(e => e.Color).HasMaxLength(3);
            entity.Property(e => e.ColorBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreationDate).HasColumnType("datetime");
            entity.Property(e => e.Dept).HasMaxLength(8);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.LinkId).HasColumnName("LinkID");
            entity.Property(e => e.ProductCode).HasMaxLength(25);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.SubType).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(25);
            entity.Property(e => e.UofM)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.WorkCtr).HasMaxLength(20);

            entity.HasOne(d => d.Product).WithMany(p => p.AIcProdBoms)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_aIC_ProdBom__IC_ProductCatalog");
        });

        modelBuilder.Entity<IcItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__IC_Item");

            entity.ToTable("_IC_Items", "dbo");

            entity.HasIndex(e => e.Item, "IX__IC_Item").IsUnique();

            entity.Property(e => e.ItemId)
                .ValueGeneratedNever()
                .HasColumnName("ItemID");
            entity.Property(e => e.CatalogNbr).HasMaxLength(50);
            entity.Property(e => e.ChangeDate).HasColumnType("smalldatetime");
            entity.Property(e => e.ChangedById).HasColumnName("ChangedByID");
            entity.Property(e => e.ColorBy).HasMaxLength(50);
            entity.Property(e => e.ColorPosInName).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.CreationDate).HasColumnType("smalldatetime");
            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.Directional).HasMaxLength(50);
            entity.Property(e => e.Item).HasMaxLength(50);
            entity.Property(e => e.ItemDepth).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ItemFin).HasMaxLength(50);
            entity.Property(e => e.ItemHeight).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ItemWeight)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("itemWeight");
            entity.Property(e => e.ItemWidth).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.PartLocation).HasMaxLength(50);
            entity.Property(e => e.PriceCategory).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ProductCatalog).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ProductCategory)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.ProductSubCategory)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.RecordDate).HasColumnType("smalldatetime");
            entity.Property(e => e.ScrapFactor).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.SizeDivisor).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UofM).HasMaxLength(50);
            entity.Property(e => e.WorkCtr).HasMaxLength(50);
        });

        modelBuilder.Entity<IcProductCatalog>(entity =>
        {
            entity.HasKey(e => e.ProductId);

            entity.ToTable("_IC_ProductCatalog", "dbo");

            entity.Property(e => e.ProductId)
                .ValueGeneratedNever()
                .HasColumnName("ProductID");
            entity.Property(e => e.Asset2D).HasMaxLength(50);
            entity.Property(e => e.Asset3D).HasMaxLength(50);
            entity.Property(e => e.AssetElv).HasMaxLength(50);
            entity.Property(e => e.CatalogPage).HasMaxLength(10);
            entity.Property(e => e.Category01).HasMaxLength(100);
            entity.Property(e => e.Category02).HasMaxLength(100);
            entity.Property(e => e.Category03).HasMaxLength(100);
            entity.Property(e => e.Category04).HasMaxLength(100);
            entity.Property(e => e.Category05).HasMaxLength(100);
            entity.Property(e => e.Category06).HasMaxLength(100);
            entity.Property(e => e.Category07).HasMaxLength(100);
            entity.Property(e => e.Category08).HasMaxLength(100);
            entity.Property(e => e.Category09).HasMaxLength(100);
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Cet).HasColumnName("CET");
            entity.Property(e => e.ChangeDate).HasColumnType("datetime");
            entity.Property(e => e.Color).HasMaxLength(3);
            entity.Property(e => e.ColorBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ColorReqd).HasMaxLength(15);
            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.CostRecordDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedById).HasColumnName("CreatedByID");
            entity.Property(e => e.CreationDate).HasColumnType("datetime");
            entity.Property(e => e.CurrentUcost).HasColumnName("CurrentUCost");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Description2).HasMaxLength(500);
            entity.Property(e => e.DwgItemNbr).HasMaxLength(50);
            entity.Property(e => e.Finish).HasMaxLength(3);
            entity.Property(e => e.HeightOffset).HasColumnName("Height_Offset");
            entity.Property(e => e.IsPackingListForPreAssembledFrames).HasColumnName("is_Packing_List_For_PreAssembled_Frames");
            entity.Property(e => e.ListPrice2018Iccdic).HasColumnName("ListPrice2018_ICCDIC");
            entity.Property(e => e.ListPrice2020ConMs).HasColumnName("ListPrice2020ConMS");
            entity.Property(e => e.ListPrice2020Nj).HasColumnName("ListPrice2020NJ");
            entity.Property(e => e.ListPrice2020con10Esc).HasColumnName("ListPrice2020con10ESC");
            entity.Property(e => e.ListPrice2020conMapt).HasColumnName("ListPrice2020conMAPT");
            entity.Property(e => e.ListPrice2021conAepa).HasColumnName("ListPrice2021conAEPA");
            entity.Property(e => e.ListPrice2021conDpd).HasColumnName("ListPrice2021conDPD");
            entity.Property(e => e.ListPrice2021conEscnj).HasColumnName("ListPrice2021conESCNJ");
            entity.Property(e => e.ListPrice2021conEscnj2).HasColumnName("ListPrice2021conESCNJ2");
            entity.Property(e => e.ListPrice2021conKcda).HasColumnName("ListPrice2021conKCDA");
            entity.Property(e => e.ListPrice2021conMs).HasColumnName("ListPrice2021conMS");
            entity.Property(e => e.ListPrice2021conNc).HasColumnName("ListPrice2021conNC");
            entity.Property(e => e.ListPrice2021conNynj).HasColumnName("ListPrice2021conNYNJ");
            entity.Property(e => e.ListPrice2021conPa).HasColumnName("ListPrice2021conPA");
            entity.Property(e => e.ListPrice2021conPeppm).HasColumnName("ListPrice2021conPEPPM");
            entity.Property(e => e.ListPrice2021conSc).HasColumnName("ListPrice2021conSC");
            entity.Property(e => e.ListPrice2021sur2Ny).HasColumnName("ListPrice2021sur2NY");
            entity.Property(e => e.ListPrice202223conEscnj).HasColumnName("ListPrice2022_23conESCNJ");
            entity.Property(e => e.ListPrice2022MeTeor).HasColumnName("ListPrice2022_MeTEOR");
            entity.Property(e => e.ListPrice2022Nbf).HasColumnName("ListPrice2022_NBF");
            entity.Property(e => e.ListPrice2022conNc).HasColumnName("ListPrice2022conNC");
            entity.Property(e => e.ListPrice2022conNcepcSse).HasColumnName("ListPrice2022conNCEPC_SSE");
            entity.Property(e => e.ListPrice2022conNcepcSsi).HasColumnName("ListPrice2022conNCEPC_SSI");
            entity.Property(e => e.ListPrice2022conPeppm).HasColumnName("ListPrice2022conPEPPM");
            entity.Property(e => e.ListPrice2022conPeppm2).HasColumnName("ListPrice2022conPEPPM2");
            entity.Property(e => e.ListPrice2022conSc).HasColumnName("ListPrice2022conSC");
            entity.Property(e => e.ListPrice2022conScA).HasColumnName("ListPrice2022conSC_a");
            entity.Property(e => e.ListPrice2023AtBoces).HasColumnName("ListPrice2023_AT_Boces");
            entity.Property(e => e.ListPrice2023NickersonNy).HasColumnName("ListPrice2023_Nickerson_NY");
            entity.Property(e => e.ListPrice2023Omnia).HasColumnName("ListPrice2023_Omnia");
            entity.Property(e => e.ListPrice2023Sspec).HasColumnName("ListPrice2023_SSpec");
            entity.Property(e => e.ListPrice2023conAepa).HasColumnName("ListPrice2023conAEPA");
            entity.Property(e => e.ListPrice2023conEscnj).HasColumnName("ListPrice2023conESCNJ");
            entity.Property(e => e.ListPrice2023conIuc).HasColumnName("ListPrice2023conIUC");
            entity.Property(e => e.ListPrice2023conKcda).HasColumnName("ListPrice2023conKCDA");
            entity.Property(e => e.ListPrice2023conNc).HasColumnName("ListPrice2023conNC");
            entity.Property(e => e.ListPrice2023conNy).HasColumnName("ListPrice2023conNY");
            entity.Property(e => e.ListPrice2023conPeppm).HasColumnName("ListPrice2023conPEPPM");
            entity.Property(e => e.ListPrice2023conSc).HasColumnName("ListPrice2023conSC");
            entity.Property(e => e.ListPrice2024Sspec).HasColumnName("ListPrice2024_SSpec");
            entity.Property(e => e.Option01).HasMaxLength(50);
            entity.Property(e => e.Option02).HasMaxLength(50);
            entity.Property(e => e.Option03).HasMaxLength(50);
            entity.Property(e => e.Option04).HasMaxLength(50);
            entity.Property(e => e.Option05).HasMaxLength(50);
            entity.Property(e => e.Option06).HasMaxLength(50);
            entity.Property(e => e.Option07).HasMaxLength(50);
            entity.Property(e => e.Option08).HasMaxLength(50);
            entity.Property(e => e.Option09).HasMaxLength(50);
            entity.Property(e => e.Option10).HasMaxLength(50);
            entity.Property(e => e.PricingComment).HasMaxLength(512);
            entity.Property(e => e.ProductCode).HasMaxLength(25);
            entity.Property(e => e.QuoteId).HasColumnName("QuoteID");
            entity.Property(e => e.QuoteItemNbr).HasMaxLength(10);
            entity.Property(e => e.QuoteNbr).HasMaxLength(12);
            entity.Property(e => e.QuotePartId).HasColumnName("QuotePartID");
            entity.Property(e => e.QuotePtNbr)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.QuoteRev)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ShouldShowInManufacturerReport).HasColumnName("should_Show_In_Manufacturer_Report");
            entity.Property(e => e.Status).HasMaxLength(10);
            entity.Property(e => e.StatusOld)
                .HasMaxLength(10)
                .HasColumnName("Status_old");
            entity.Property(e => e.StdPriceGrade)
                .HasMaxLength(3)
                .IsFixedLength();
            entity.Property(e => e.SubType).HasMaxLength(50);
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.ThreeDlayerName)
                .HasMaxLength(50)
                .HasColumnName("threeDLayerName");
            entity.Property(e => e.Type).HasMaxLength(25);
            entity.Property(e => e.UnspscCode)
                .HasMaxLength(10)
                .HasColumnName("UNSPSC_Code");
            entity.Property(e => e.UofM)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.VircoUscItem).HasColumnName("VircoUSC_item");
            entity.Property(e => e.WsTedgeUsage).HasColumnName("WS_Tedge_Usage");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
