using System;
using System.Collections.Generic;

namespace OMPS.Models.Order;

public partial class AIcColorSet
{
    public Guid ColorSetId { get; set; }

    public string Name { get; set; } = null!;

    public string OrderNumber { get; set; } = null!;

    public int LineNumber { get; set; }

    public string? OpportunityNbr { get; set; }

    public string? QuoteNbr { get; set; }

    public Guid? QuoteId { get; set; }

    public DateTime? OrderDate { get; set; }

    public double Qty { get; set; }

    public string? LineItem { get; set; }

    public string? LineDescription { get; set; }

    public string? SupplyOrderRef { get; set; }

    public string? SupplyOrderRefLine { get; set; }

    public string? CreatedByName { get; set; }

    public Guid? CreatedById { get; set; }

    public string? CompanyName { get; set; }

    public string? ShiptoName { get; set; }

    public string? DealerName { get; set; }

    public string? DealerRep { get; set; }

    public string? OrderType { get; set; }

    public string? ColorSetDescription { get; set; }

    public string? ColorScheme { get; set; }

    public bool InstallByIc { get; set; }

    public string? InstallbyIcc { get; set; }

    public string? FrameFin { get; set; }

    public string? PmoldFin { get; set; }

    public string? MetalFin { get; set; }

    public string? ShelfFin { get; set; }

    public string? AccFin { get; set; }

    public string? EdgeFin { get; set; }

    public string? TblBaseFin { get; set; }

    public string? TblLegFin { get; set; }

    public string? TblLegBaseFin { get; set; }

    public string? TblSlimLegFin { get; set; }

    public string? TangentLegFin { get; set; }

    public Guid? Fabric1Id { get; set; }

    public Guid? Fabric2Id { get; set; }

    public Guid? Fabric3Id { get; set; }

    public Guid? WksLamId { get; set; }

    public Guid? WksLam2Id { get; set; }

    public Guid? WksEdgeId { get; set; }

    public Guid? WksEdge75Id { get; set; }

    public Guid? EdgeBand75Id { get; set; }

    public Guid? TflpnlId { get; set; }

    public Guid? Tflpnl2Id { get; set; }

    public Guid? TflpnlModestyId { get; set; }

    public Guid? LamPnlId { get; set; }

    public Guid? LamPnl2Id { get; set; }

    public Guid? LamCurveId { get; set; }

    public Guid? LamHcdoorId { get; set; }

    public Guid? ChaseDoorId { get; set; }

    public Guid? AcrylicId { get; set; }

    public Guid? Acrylic2Id { get; set; }

    public string? MiscFinish1Type { get; set; }

    public Guid? MiscFinish1Id { get; set; }

    public string? MiscFinish2Type { get; set; }

    public Guid? MiscFinish2Id { get; set; }

    public string? PedCushionColor { get; set; }

    public string? UnderWsPnlType { get; set; }

    public int? WorksurfaceHeight { get; set; }

    public bool Xq { get; set; }

    public bool KeyAlike { get; set; }

    public bool MasterKey { get; set; }

    public bool KeyEaStaAlike { get; set; }

    public bool EarlyElectrical { get; set; }

    public string? Notes { get; set; }

    public DateTime? ShipByDate { get; set; }

    public DateTime? OnSiteDate { get; set; }

    public double? NetProduct { get; set; }

    public double DealerPayable { get; set; }

    public double InvoiceAmt { get; set; }

    public bool ImportQuote { get; set; }

    public DateTime ChangeDate { get; set; }

    public DateTime CreationDate { get; set; }

    public Guid ChangedbyId { get; set; }

    public bool Preengined { get; set; }

    public bool Engined { get; set; }

    public bool Calcd { get; set; }

    public string? Engineer { get; set; }

    public bool? NetProd { get; set; }

    public bool Part01 { get; set; }

    public bool Part02 { get; set; }

    public bool Part03 { get; set; }

    public bool Part04 { get; set; }

    public bool Part05 { get; set; }

    public bool Part06 { get; set; }

    public bool Part07 { get; set; }

    public bool Part08 { get; set; }

    public bool Part09 { get; set; }

    public bool Part10 { get; set; }

    public bool Part11 { get; set; }

    public bool Part12 { get; set; }

    public bool Part13 { get; set; }

    public bool Part14 { get; set; }

    public bool Part15 { get; set; }

    public bool Part16 { get; set; }

    public bool Part17 { get; set; }

    public bool Part18 { get; set; }

    public bool Part19 { get; set; }

    public bool Part20 { get; set; }

    public bool Part21 { get; set; }

    public bool Part22 { get; set; }

    public bool Part23 { get; set; }

    public bool Part24 { get; set; }

    public bool Part25 { get; set; }

    public bool Part26 { get; set; }

    public bool Part27 { get; set; }

    public bool Part28 { get; set; }

    public bool Part29 { get; set; }

    public bool Part30 { get; set; }

    public bool Part31 { get; set; }

    public bool Part32 { get; set; }

    public bool Part33 { get; set; }

    public bool Part34 { get; set; }

    public bool Part35 { get; set; }

    public bool Part36 { get; set; }

    public bool Part37 { get; set; }

    public bool Part38 { get; set; }

    public bool Part39 { get; set; }

    public bool Part40 { get; set; }

    public bool Part41 { get; set; }

    public bool Part42 { get; set; }

    public bool Part43 { get; set; }

    public bool Part44 { get; set; }

    public bool Part45 { get; set; }

    public bool Part46 { get; set; }

    public bool Part47 { get; set; }

    public bool Part48 { get; set; }

    public bool Part49 { get; set; }

    public bool Part50 { get; set; }

    public bool Part51 { get; set; }

    public bool Part52 { get; set; }

    public bool Part53 { get; set; }

    public bool Part54 { get; set; }

    public bool Part55 { get; set; }

    public bool Part56 { get; set; }

    public bool Part57 { get; set; }

    public bool Part58 { get; set; }

    public bool Part59 { get; set; }

    public bool Part60 { get; set; }

    public bool Part61 { get; set; }

    public bool Part62 { get; set; }

    public bool Part63 { get; set; }

    public bool Part64 { get; set; }

    public bool Part65 { get; set; }

    public bool Part66 { get; set; }

    public bool Part67 { get; set; }

    public bool Part68 { get; set; }

    public bool Part69 { get; set; }

    public bool Part70 { get; set; }

    public bool Part71 { get; set; }

    public bool Part72 { get; set; }

    public bool Part73 { get; set; }

    public bool Part74 { get; set; }

    public bool Part75 { get; set; }

    public bool Part76 { get; set; }

    public bool Part77 { get; set; }

    public bool Part78 { get; set; }

    public bool Part79 { get; set; }

    public bool Part80 { get; set; }

    public bool Part81 { get; set; }

    public bool Part82 { get; set; }

    public bool Part83 { get; set; }

    public bool Part84 { get; set; }

    public bool Part85 { get; set; }

    public bool Part86 { get; set; }

    public bool Part87 { get; set; }

    public bool Part88 { get; set; }

    public bool Part89 { get; set; }

    public bool Part90 { get; set; }

    public bool Part91 { get; set; }

    public bool Part92 { get; set; }

    public bool Part93 { get; set; }

    public bool Part94 { get; set; }

    public bool Part95 { get; set; }

    public bool UpdateRecord { get; set; }

    public int? JobDoors { get; set; }

    public int? JobFrames { get; set; }

    public int? JobTubes { get; set; }

    public int? JobPmold { get; set; }

    public int? JobPanels { get; set; }

    public int? JobFabricSides { get; set; }

    public int? JobPartsNote { get; set; }

    public int? JobProductionNote { get; set; }

    public string? JobTagColor { get; set; }

    public int? JobWorksurfaces { get; set; }

    public int? JobHcdoors { get; set; }

    public DateTime? JobStartDate { get; set; }

    public DateTime? JobEndDate { get; set; }

    public int? JobWksSheets { get; set; }

    public int? JobPnlSheets { get; set; }
}
