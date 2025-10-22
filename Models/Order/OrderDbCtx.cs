using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;

namespace OMPS.Models.Order;

public partial class OrderDbCtx : DbContext
{
    public OrderDbCtx()
    {
    }

    public OrderDbCtx(DbContextOptions<OrderDbCtx> options)
        : base(options)
    {
    }

    public virtual DbSet<AIcColorSet> AIcColorSets { get; set; }

    public virtual DbSet<AIcIceManuf> AIcIceManufs { get; set; }

    public virtual DbSet<AIcIceManufPart> AIcIceManufParts { get; set; }

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

        modelBuilder.Entity<AIcIceManufPart>(entity =>
        {
            entity.HasKey(e => e.IceManufPartId);

            entity.ToTable("aIC_IceManufPart", "dbo");

            entity.Property(e => e.IceManufPartId)
                .ValueGeneratedNever()
                .HasColumnName("IceManufPartID");
            entity.Property(e => e.ChangeDate).HasColumnType("datetime");
            entity.Property(e => e.ChangedbyId).HasColumnName("ChangedbyID");
            entity.Property(e => e.ColorSetId).HasColumnName("ColorSetID");
            entity.Property(e => e.CreationDate).HasColumnType("datetime");
            entity.Property(e => e.IceManufId).HasColumnName("IceManufID");
            entity.Property(e => e.PartDescription).HasMaxLength(50);
            entity.Property(e => e.PartNbr).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
