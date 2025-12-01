using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace OMPS.DBModels.Order;

public partial class AIcColorSet: ObservableObject
{
    [ObservableProperty]
    public partial Guid ColorSetId { get; set; }

    [ObservableProperty]
	public partial string Name { get; set; } = null!;

    [ObservableProperty]
	public partial string OrderNumber { get; set; } = null!;

    [ObservableProperty]
	public partial int LineNumber { get; set; }

    [ObservableProperty]
	public partial string? OpportunityNbr { get; set; }

    [ObservableProperty]
	public partial string? QuoteNbr { get; set; }

    [ObservableProperty]
	public partial Guid? QuoteId { get; set; }

    [ObservableProperty]
	public partial DateTime? OrderDate { get; set; }

    [ObservableProperty]
	public partial double Qty { get; set; }

    [ObservableProperty]
	public partial string? LineItem { get; set; }

    [ObservableProperty]
	public partial string? LineDescription { get; set; }

    [ObservableProperty]
	public partial string? SupplyOrderRef { get; set; }

    [ObservableProperty]
	public partial string? SupplyOrderRefLine { get; set; }

    [ObservableProperty]
	public partial string? CreatedByName { get; set; }

    [ObservableProperty]
	public partial Guid? CreatedById { get; set; }

    [ObservableProperty]
	public partial string? CompanyName { get; set; }

    [ObservableProperty]
	public partial string? ShiptoName { get; set; }

    [ObservableProperty]
	public partial string? DealerName { get; set; }

    [ObservableProperty]
	public partial string? DealerRep { get; set; }

    [ObservableProperty]
	public partial string? OrderType { get; set; }

    [ObservableProperty]
	public partial string? ColorSetDescription { get; set; }

    [ObservableProperty]
	public partial string? ColorScheme { get; set; }

    [ObservableProperty]
	public partial bool InstallByIc { get; set; }

    [ObservableProperty]
	public partial string? InstallbyIcc { get; set; }

    [ObservableProperty]
	public partial string? FrameFin { get; set; }

    [ObservableProperty]
	public partial string? PmoldFin { get; set; }

    [ObservableProperty]
	public partial string? MetalFin { get; set; }

    [ObservableProperty]
	public partial string? ShelfFin { get; set; }

    [ObservableProperty]
	public partial string? AccFin { get; set; }

    [ObservableProperty]
	public partial string? EdgeFin { get; set; }

    [ObservableProperty]
	public partial string? TblBaseFin { get; set; }

    [ObservableProperty]
	public partial string? TblLegFin { get; set; }

    [ObservableProperty]
	public partial string? TblLegBaseFin { get; set; }

    [ObservableProperty]
	public partial string? TblSlimLegFin { get; set; }

    [ObservableProperty]
	public partial string? TangentLegFin { get; set; }

    [ObservableProperty]
	public partial Guid? Fabric1Id { get; set; }

    [ObservableProperty]
	public partial Guid? Fabric2Id { get; set; }

    [ObservableProperty]
	public partial Guid? Fabric3Id { get; set; }

    [ObservableProperty]
	public partial Guid? WksLamId { get; set; }

    [ObservableProperty]
	public partial Guid? WksLam2Id { get; set; }

    [ObservableProperty]
	public partial Guid? WksEdgeId { get; set; }

    [ObservableProperty]
	public partial Guid? WksEdge75Id { get; set; }

    [ObservableProperty]
	public partial Guid? EdgeBand75Id { get; set; }

    [ObservableProperty]
	public partial Guid? TflpnlId { get; set; }

    [ObservableProperty]
	public partial Guid? Tflpnl2Id { get; set; }

    [ObservableProperty]
	public partial Guid? TflpnlModestyId { get; set; }

    [ObservableProperty]
	public partial Guid? LamPnlId { get; set; }

    [ObservableProperty]
	public partial Guid? LamPnl2Id { get; set; }

    [ObservableProperty]
	public partial Guid? LamCurveId { get; set; }

    [ObservableProperty]
	public partial Guid? LamHcdoorId { get; set; }

    [ObservableProperty]
	public partial Guid? ChaseDoorId { get; set; }

    [ObservableProperty]
	public partial Guid? AcrylicId { get; set; }

    [ObservableProperty]
	public partial Guid? Acrylic2Id { get; set; }

    [ObservableProperty]
	public partial string? MiscFinish1Type { get; set; }

    [ObservableProperty]
	public partial Guid? MiscFinish1Id { get; set; }

    [ObservableProperty]
	public partial string? MiscFinish2Type { get; set; }

    [ObservableProperty]
	public partial Guid? MiscFinish2Id { get; set; }

    [ObservableProperty]
	public partial string? PedCushionColor { get; set; }

    [ObservableProperty]
	public partial string? UnderWsPnlType { get; set; }

    [ObservableProperty]
	public partial int? WorksurfaceHeight { get; set; }

    [ObservableProperty]
	public partial bool Xq { get; set; }

    [ObservableProperty]
	public partial bool KeyAlike { get; set; }

    [ObservableProperty]
	public partial bool MasterKey { get; set; }

    [ObservableProperty]
	public partial bool KeyEaStaAlike { get; set; }

    [ObservableProperty]
	public partial bool EarlyElectrical { get; set; }

    [ObservableProperty]
	public partial string? Notes { get; set; }

    [ObservableProperty]
	public partial DateTime? ShipByDate { get; set; }

    [ObservableProperty]
	public partial DateTime? OnSiteDate { get; set; }

    [ObservableProperty]
	public partial double? NetProduct { get; set; }

    [ObservableProperty]
	public partial double DealerPayable { get; set; }

    [ObservableProperty]
	public partial double InvoiceAmt { get; set; }

    [ObservableProperty]
	public partial bool ImportQuote { get; set; }

    [ObservableProperty]
	public partial DateTime ChangeDate { get; set; }

    [ObservableProperty]
	public partial DateTime CreationDate { get; set; }

    [ObservableProperty]
	public partial Guid ChangedbyId { get; set; }

    [ObservableProperty]
	public partial bool Preengined { get; set; }

    [ObservableProperty]
	public partial bool Engined { get; set; }

    [ObservableProperty]
	public partial bool Calcd { get; set; }

    [ObservableProperty]
	public partial string? Engineer { get; set; }

    [ObservableProperty]
	public partial bool? NetProd { get; set; }

    [ObservableProperty]
	public partial bool Part01 { get; set; }

    [ObservableProperty]
	public partial bool Part02 { get; set; }

    [ObservableProperty]
	public partial bool Part03 { get; set; }

    [ObservableProperty]
	public partial bool Part04 { get; set; }

    [ObservableProperty]
	public partial bool Part05 { get; set; }

    [ObservableProperty]
	public partial bool Part06 { get; set; }

    [ObservableProperty]
	public partial bool Part07 { get; set; }

    [ObservableProperty]
	public partial bool Part08 { get; set; }

    [ObservableProperty]
	public partial bool Part09 { get; set; }

    [ObservableProperty]
	public partial bool Part10 { get; set; }

    [ObservableProperty]
	public partial bool Part11 { get; set; }

    [ObservableProperty]
	public partial bool Part12 { get; set; }

    [ObservableProperty]
	public partial bool Part13 { get; set; }

    [ObservableProperty]
	public partial bool Part14 { get; set; }

    [ObservableProperty]
	public partial bool Part15 { get; set; }

    [ObservableProperty]
	public partial bool Part16 { get; set; }

    [ObservableProperty]
	public partial bool Part17 { get; set; }

    [ObservableProperty]
	public partial bool Part18 { get; set; }

    [ObservableProperty]
	public partial bool Part19 { get; set; }

    [ObservableProperty]
	public partial bool Part20 { get; set; }

    [ObservableProperty]
	public partial bool Part21 { get; set; }

    [ObservableProperty]
	public partial bool Part22 { get; set; }

    [ObservableProperty]
	public partial bool Part23 { get; set; }

    [ObservableProperty]
	public partial bool Part24 { get; set; }

    [ObservableProperty]
	public partial bool Part25 { get; set; }

    [ObservableProperty]
	public partial bool Part26 { get; set; }

    [ObservableProperty]
	public partial bool Part27 { get; set; }

    [ObservableProperty]
	public partial bool Part28 { get; set; }

    [ObservableProperty]
	public partial bool Part29 { get; set; }

    [ObservableProperty]
	public partial bool Part30 { get; set; }

    [ObservableProperty]
	public partial bool Part31 { get; set; }

    [ObservableProperty]
	public partial bool Part32 { get; set; }

    [ObservableProperty]
	public partial bool Part33 { get; set; }

    [ObservableProperty]
	public partial bool Part34 { get; set; }

    [ObservableProperty]
	public partial bool Part35 { get; set; }

    [ObservableProperty]
	public partial bool Part36 { get; set; }

    [ObservableProperty]
	public partial bool Part37 { get; set; }

    [ObservableProperty]
	public partial bool Part38 { get; set; }

    [ObservableProperty]
	public partial bool Part39 { get; set; }

    [ObservableProperty]
	public partial bool Part40 { get; set; }

    [ObservableProperty]
	public partial bool Part41 { get; set; }

    [ObservableProperty]
	public partial bool Part42 { get; set; }

    [ObservableProperty]
	public partial bool Part43 { get; set; }

    [ObservableProperty]
	public partial bool Part44 { get; set; }

    [ObservableProperty]
	public partial bool Part45 { get; set; }

    [ObservableProperty]
	public partial bool Part46 { get; set; }

    [ObservableProperty]
	public partial bool Part47 { get; set; }

    [ObservableProperty]
	public partial bool Part48 { get; set; }

    [ObservableProperty]
	public partial bool Part49 { get; set; }

    [ObservableProperty]
	public partial bool Part50 { get; set; }

    [ObservableProperty]
	public partial bool Part51 { get; set; }

    [ObservableProperty]
	public partial bool Part52 { get; set; }

    [ObservableProperty]
	public partial bool Part53 { get; set; }

    [ObservableProperty]
	public partial bool Part54 { get; set; }

    [ObservableProperty]
	public partial bool Part55 { get; set; }

    [ObservableProperty]
	public partial bool Part56 { get; set; }

    [ObservableProperty]
	public partial bool Part57 { get; set; }

    [ObservableProperty]
	public partial bool Part58 { get; set; }

    [ObservableProperty]
	public partial bool Part59 { get; set; }

    [ObservableProperty]
	public partial bool Part60 { get; set; }

    [ObservableProperty]
	public partial bool Part61 { get; set; }

    [ObservableProperty]
	public partial bool Part62 { get; set; }

    [ObservableProperty]
	public partial bool Part63 { get; set; }

    [ObservableProperty]
	public partial bool Part64 { get; set; }

    [ObservableProperty]
	public partial bool Part65 { get; set; }

    [ObservableProperty]
	public partial bool Part66 { get; set; }

    [ObservableProperty]
	public partial bool Part67 { get; set; }

    [ObservableProperty]
	public partial bool Part68 { get; set; }

    [ObservableProperty]
	public partial bool Part69 { get; set; }

    [ObservableProperty]
	public partial bool Part70 { get; set; }

    [ObservableProperty]
	public partial bool Part71 { get; set; }

    [ObservableProperty]
	public partial bool Part72 { get; set; }

    [ObservableProperty]
	public partial bool Part73 { get; set; }

    [ObservableProperty]
	public partial bool Part74 { get; set; }

    [ObservableProperty]
	public partial bool Part75 { get; set; }

    [ObservableProperty]
	public partial bool Part76 { get; set; }

    [ObservableProperty]
	public partial bool Part77 { get; set; }

    [ObservableProperty]
	public partial bool Part78 { get; set; }

    [ObservableProperty]
	public partial bool Part79 { get; set; }

    [ObservableProperty]
	public partial bool Part80 { get; set; }

    [ObservableProperty]
	public partial bool Part81 { get; set; }

    [ObservableProperty]
	public partial bool Part82 { get; set; }

    [ObservableProperty]
	public partial bool Part83 { get; set; }

    [ObservableProperty]
	public partial bool Part84 { get; set; }

    [ObservableProperty]
	public partial bool Part85 { get; set; }

    [ObservableProperty]
	public partial bool Part86 { get; set; }

    [ObservableProperty]
	public partial bool Part87 { get; set; }

    [ObservableProperty]
	public partial bool Part88 { get; set; }

    [ObservableProperty]
	public partial bool Part89 { get; set; }

    [ObservableProperty]
	public partial bool Part90 { get; set; }

    [ObservableProperty]
	public partial bool Part91 { get; set; }

    [ObservableProperty]
	public partial bool Part92 { get; set; }

    [ObservableProperty]
	public partial bool Part93 { get; set; }

    [ObservableProperty]
	public partial bool Part94 { get; set; }

    [ObservableProperty]
	public partial bool Part95 { get; set; }

    [ObservableProperty]
	public partial bool UpdateRecord { get; set; }

    [ObservableProperty]
	public partial int? JobDoors { get; set; }

    [ObservableProperty]
	public partial int? JobFrames { get; set; }

    [ObservableProperty]
	public partial int? JobTubes { get; set; }

    [ObservableProperty]
	public partial int? JobPmold { get; set; }

    [ObservableProperty]
	public partial int? JobPanels { get; set; }

    [ObservableProperty]
	public partial int? JobFabricSides { get; set; }

    [ObservableProperty]
	public partial int? JobPartsNote { get; set; }

    [ObservableProperty]
	public partial int? JobProductionNote { get; set; }

    [ObservableProperty]
	public partial string? JobTagColor { get; set; }

    [ObservableProperty]
	public partial int? JobWorksurfaces { get; set; }

    [ObservableProperty]
	public partial int? JobHcdoors { get; set; }

    [ObservableProperty]
	public partial DateTime? JobStartDate { get; set; }

    [ObservableProperty]
	public partial DateTime? JobEndDate { get; set; }

    [ObservableProperty]
	public partial int? JobWksSheets { get; set; }

    [ObservableProperty]
	public partial int? JobPnlSheets { get; set; }
}
