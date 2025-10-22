using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace OMPS.Models;

public partial class IcEmqContext : DbContext
{
    public IcEmqContext()
    {
    }

    public IcEmqContext(DbContextOptions<IcEmqContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AIcColorSet> AIcColorSets { get; set; }

    public virtual DbSet<AIcIceManuf> AIcIceManufs { get; set; }

    public virtual DbSet<IcProductCatalog> IcProductCatalogs { get; set; }

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
        modelBuilder.Entity<AIcColorSet>(entity =>
        {
            entity.HasKey(e => e.ColorSetId);

            entity.ToTable("aIC_ColorSet", "dbo");

            entity.HasIndex(e => e.Name, "IX_aIC_ColorSet").IsUnique();

            entity.HasIndex(e => e.OrderNumber, "IX_aIC_ColorSet_1");

            entity.HasIndex(e => e.LineNumber, "IX_aIC_ColorSet_2");

            entity.Property(e => e.ColorSetId)
                .ValueGeneratedNever()
                .HasColumnName("ColorSetID");
            entity.Property(e => e.AccFin)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.Acrylic2Id).HasColumnName("Acrylic2ID");
            entity.Property(e => e.AcrylicId).HasColumnName("AcrylicID");
            entity.Property(e => e.ChangeDate).HasColumnType("datetime");
            entity.Property(e => e.ChangedbyId).HasColumnName("ChangedbyID");
            entity.Property(e => e.ChaseDoorId).HasColumnName("ChaseDoorID");
            entity.Property(e => e.ColorScheme)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.ColorSetDescription).HasMaxLength(250);
            entity.Property(e => e.CompanyName).HasMaxLength(60);
            entity.Property(e => e.CreatedById).HasColumnName("CreatedByID");
            entity.Property(e => e.CreatedByName).HasMaxLength(50);
            entity.Property(e => e.CreationDate).HasColumnType("datetime");
            entity.Property(e => e.DealerName).HasMaxLength(60);
            entity.Property(e => e.DealerRep).HasMaxLength(50);
            entity.Property(e => e.EdgeBand75Id).HasColumnName("EdgeBand75ID");
            entity.Property(e => e.EdgeFin)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.Engineer).HasMaxLength(50);
            entity.Property(e => e.Fabric1Id).HasColumnName("Fabric1ID");
            entity.Property(e => e.Fabric2Id).HasColumnName("Fabric2ID");
            entity.Property(e => e.Fabric3Id).HasColumnName("Fabric3ID");
            entity.Property(e => e.FrameFin)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.InstallByIc).HasColumnName("InstallByIC");
            entity.Property(e => e.InstallbyIcc)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("InstallbyICC");
            entity.Property(e => e.JobDoors).HasColumnName("Job_Doors");
            entity.Property(e => e.JobEndDate)
                .HasColumnType("datetime")
                .HasColumnName("Job_EndDate");
            entity.Property(e => e.JobFabricSides).HasColumnName("Job_FabricSides");
            entity.Property(e => e.JobFrames).HasColumnName("Job_Frames");
            entity.Property(e => e.JobHcdoors).HasColumnName("Job_HCDoors");
            entity.Property(e => e.JobPanels).HasColumnName("Job_Panels");
            entity.Property(e => e.JobPartsNote).HasColumnName("Job_PartsNote");
            entity.Property(e => e.JobPmold).HasColumnName("Job_Pmold");
            entity.Property(e => e.JobPnlSheets).HasColumnName("Job_PnlSheets");
            entity.Property(e => e.JobProductionNote).HasColumnName("Job_ProductionNote");
            entity.Property(e => e.JobStartDate)
                .HasColumnType("datetime")
                .HasColumnName("Job_StartDate");
            entity.Property(e => e.JobTagColor)
                .HasMaxLength(50)
                .HasColumnName("Job_TagColor");
            entity.Property(e => e.JobTubes).HasColumnName("Job_Tubes");
            entity.Property(e => e.JobWksSheets).HasColumnName("Job_WksSheets");
            entity.Property(e => e.JobWorksurfaces).HasColumnName("Job_Worksurfaces");
            entity.Property(e => e.LamCurveId).HasColumnName("LamCurveID");
            entity.Property(e => e.LamHcdoorId).HasColumnName("LamHCDoorID");
            entity.Property(e => e.LamPnl2Id).HasColumnName("LamPnl2ID");
            entity.Property(e => e.LamPnlId).HasColumnName("LamPnlID");
            entity.Property(e => e.LineDescription).HasMaxLength(50);
            entity.Property(e => e.LineItem).HasMaxLength(50);
            entity.Property(e => e.MetalFin)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.MiscFinish1Id).HasColumnName("MiscFinish1ID");
            entity.Property(e => e.MiscFinish1Type).HasMaxLength(25);
            entity.Property(e => e.MiscFinish2Id).HasColumnName("MiscFinish2ID");
            entity.Property(e => e.MiscFinish2Type).HasMaxLength(25);
            entity.Property(e => e.Name).HasMaxLength(20);
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnName("notes");
            entity.Property(e => e.OnSiteDate).HasColumnType("smalldatetime");
            entity.Property(e => e.OpportunityNbr).HasMaxLength(20);
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.OrderNumber).HasMaxLength(20);
            entity.Property(e => e.OrderType).HasMaxLength(50);
            entity.Property(e => e.PedCushionColor).HasMaxLength(50);
            entity.Property(e => e.PmoldFin)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.QuoteId).HasColumnName("QuoteID");
            entity.Property(e => e.QuoteNbr).HasMaxLength(12);
            entity.Property(e => e.ShelfFin)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.ShipByDate).HasColumnType("smalldatetime");
            entity.Property(e => e.ShiptoName).HasMaxLength(60);
            entity.Property(e => e.SupplyOrderRef).HasMaxLength(35);
            entity.Property(e => e.SupplyOrderRefLine).HasMaxLength(35);
            entity.Property(e => e.TangentLegFin)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TblBaseFin)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.TblLegBaseFin)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.TblLegFin)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.TblSlimLegFin)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.Tflpnl2Id).HasColumnName("TFLPnl2ID");
            entity.Property(e => e.TflpnlId).HasColumnName("TFLPnlID");
            entity.Property(e => e.TflpnlModestyId).HasColumnName("TFLPnlModestyID");
            entity.Property(e => e.UnderWsPnlType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.WksEdge75Id).HasColumnName("wksEdge75ID");
            entity.Property(e => e.WksEdgeId).HasColumnName("WksEdgeID");
            entity.Property(e => e.WksLam2Id).HasColumnName("WksLam2ID");
            entity.Property(e => e.WksLamId).HasColumnName("WksLamID");
            entity.Property(e => e.Xq).HasColumnName("XQ");
        });

        modelBuilder.Entity<AIcIceManuf>(entity =>
        {
            entity.HasKey(e => e.IceManufId);

            entity.ToTable("aIC_IceManuf", "dbo");

            entity.HasIndex(e => e.ColorSetId, "IX_aIC_IceManuf");

            entity.Property(e => e.IceManufId)
                .ValueGeneratedNever()
                .HasColumnName("IceManufID");
            entity.Property(e => e.Area).HasMaxLength(50);
            entity.Property(e => e.Assembled).HasMaxLength(50);
            entity.Property(e => e.AssyNbr).HasMaxLength(50);
            entity.Property(e => e.CatalogNbr).HasMaxLength(50);
            entity.Property(e => e.ChangeDate).HasColumnType("datetime");
            entity.Property(e => e.ChangedbyId).HasColumnName("ChangedbyID");
            entity.Property(e => e.ColorBy).HasMaxLength(20);
            entity.Property(e => e.ColorSetId).HasColumnName("ColorSetID");
            entity.Property(e => e.CoreSize).HasMaxLength(50);
            entity.Property(e => e.CreatedById).HasColumnName("CreatedByID");
            entity.Property(e => e.CreationDate).HasColumnType("datetime");
            entity.Property(e => e.CustOrderNbr).HasMaxLength(50);
            entity.Property(e => e.Dept).HasMaxLength(8);
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.Idnbr)
                .HasMaxLength(50)
                .HasColumnName("IDNbr");
            entity.Property(e => e.ItemCore).HasMaxLength(50);
            entity.Property(e => e.ItemFin).HasMaxLength(50);
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.ItemNbr).HasMaxLength(50);
            entity.Property(e => e.JobNbr).HasMaxLength(50);
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
            entity.Property(e => e.PartNbr).HasMaxLength(50);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ProductLinkId).HasColumnName("ProductLinkID");
            entity.Property(e => e.QuoteNbr).HasMaxLength(50);
            entity.Property(e => e.SubType).HasMaxLength(50);
            entity.Property(e => e.TileIndicator).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UofM).HasMaxLength(50);
            entity.Property(e => e.Usertag1).HasMaxLength(150);
            entity.Property(e => e.WorkCtr).HasMaxLength(20);
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
