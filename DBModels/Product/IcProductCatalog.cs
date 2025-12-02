using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;

namespace OMPS.DBModels.Product;

public partial class IcProductCatalog: ObservableObject
{
    [ObservableProperty]
	public partial Guid ProductId { get; set; }

    [ObservableProperty]
	public partial Guid SupplierId { get; set; }

    [ObservableProperty]
	public partial Guid? CategoryId { get; set; }

    [ObservableProperty]
	public partial Guid? ContractId { get; set; }

    [ObservableProperty]
	public partial Guid? QuoteId { get; set; }

    [ObservableProperty]
	public partial Guid? QuotePartId { get; set; }

    [ObservableProperty]
	public partial string? QuoteNbr { get; set; }

    [ObservableProperty]
	public partial string? QuoteItemNbr { get; set; }

    [ObservableProperty]
	public partial string? QuotePtNbr { get; set; }

    [ObservableProperty]
	public partial string? QuoteRev { get; set; }

    [ObservableProperty]
	public partial string? CatalogPage { get; set; }

    [ObservableProperty]
	public partial string ProductCode { get; set; } = null!;

    [ObservableProperty]
	public partial string? Description { get; set; }

    [ObservableProperty]
	public partial string? Description2 { get; set; }

    [ObservableProperty]
	public partial string UofM { get; set; } = null!;

    [ObservableProperty]
	public partial string? Type { get; set; }

    [ObservableProperty]
	public partial string? SubType { get; set; }

    [ObservableProperty]
	public partial bool FurnCatalog { get; set; }

    [ObservableProperty]
	public partial bool ProjectMatrix { get; set; }

    [ObservableProperty]
	public partial bool Cet { get; set; }

    [ObservableProperty]
	public partial bool ExpressLane { get; set; }

    [ObservableProperty]
	public partial bool ExpressLaneComplete { get; set; }

    [ObservableProperty]
	public partial string? DwgItemNbr { get; set; }

    [ObservableProperty]
	public partial double StandardPrice { get; set; }

    [ObservableProperty]
	public partial double Cost { get; set; }

    [ObservableProperty]
	public partial DateTime? CostRecordDate { get; set; }

    [ObservableProperty]
	public partial Guid CreatedById { get; set; }

    [ObservableProperty]
	public partial DateTime CreationDate { get; set; }

    [ObservableProperty]
	public partial DateTime ChangeDate { get; set; }

    [ObservableProperty]
	public partial double StdNetPrice { get; set; }

    [ObservableProperty]
	public partial double Weight { get; set; }

    [ObservableProperty]
	public partial double Width { get; set; }

    [ObservableProperty]
	public partial double Depth { get; set; }

    [ObservableProperty]
	public partial double Height { get; set; }

    [ObservableProperty]
	public partial string Status { get; set; } = null!;

    [ObservableProperty]
	public partial string? StatusOld { get; set; }

    [ObservableProperty]
	public partial string? ColorBy { get; set; }

    [ObservableProperty]
	public partial string? Color { get; set; }

    [ObservableProperty]
	public partial string? Finish { get; set; }

    [ObservableProperty]
	public partial bool Explode { get; set; }

    [ObservableProperty]
	public partial bool ExplodePre { get; set; }

    [ObservableProperty]
	public partial bool PreList { get; set; }

    [ObservableProperty]
	public partial string? PricingComment { get; set; }

    [ObservableProperty]
	public partial double CostRollup { get; set; }

    [ObservableProperty]
	public partial double ListPriceRollup { get; set; }

    [ObservableProperty]
	public partial double WeightRollup { get; set; }

    [ObservableProperty]
	public partial bool RollupPrice { get; set; }

    [ObservableProperty]
	public partial double CurrentList { get; set; }

    [ObservableProperty]
	public partial double PreviousList { get; set; }

    [ObservableProperty]
	public partial bool Contract { get; set; }

    [ObservableProperty]
	public partial double ListPrice2010 { get; set; }

    [ObservableProperty]
	public partial double ListPrice2011 { get; set; }

    [ObservableProperty]
	public partial double ListPrice2012 { get; set; }

    [ObservableProperty]
	public partial double ListPrice2013 { get; set; }

    [ObservableProperty]
	public partial double ListPrice2014 { get; set; }

    [ObservableProperty]
	public partial double ListPrice2015 { get; set; }

    [ObservableProperty]
	public partial double ListPrice2016 { get; set; }

    [ObservableProperty]
	public partial double ListPrice2017 { get; set; }

    [ObservableProperty]
	public partial double ListPrice2017usc { get; set; }

    [ObservableProperty]
	public partial double ListPrice2018 { get; set; }

    [ObservableProperty]
	public partial double ListPrice2018con { get; set; }

    [ObservableProperty]
	public partial double ListPrice2018t { get; set; }

    [ObservableProperty]
	public partial double ListPrice2018Iccdic { get; set; }

    [ObservableProperty]
	public partial double ListPrice2019 { get; set; }

    [ObservableProperty]
	public partial double ListPrice2019con { get; set; }

    [ObservableProperty]
	public partial double? ListPrice2019t { get; set; }

    [ObservableProperty]
	public partial double ListPrice2020 { get; set; }

    [ObservableProperty]
	public partial double ListPrice2020Nj { get; set; }

    [ObservableProperty]
	public partial double ListPrice2020Con { get; set; }

    [ObservableProperty]
	public partial double ListPrice2020ConA { get; set; }

    [ObservableProperty]
	public partial double ListPrice2020ConMs { get; set; }

    [ObservableProperty]
	public partial double ListPrice2020conMapt { get; set; }

    [ObservableProperty]
	public partial double ListPrice2020con10Esc { get; set; }

    [ObservableProperty]
	public partial double ListPrice2020Temp { get; set; }

    [ObservableProperty]
	public partial double ListPrice2021 { get; set; }

    [ObservableProperty]
	public partial double ListPrice2021conDpd { get; set; }

    [ObservableProperty]
	public partial double ListPrice2021conMs { get; set; }

    [ObservableProperty]
	public partial double ListPrice2021conNc { get; set; }

    [ObservableProperty]
	public partial double ListPrice2021conNynj { get; set; }

    [ObservableProperty]
	public partial double ListPrice2021conPa { get; set; }

    [ObservableProperty]
	public partial double ListPrice2021conPeppm { get; set; }

    [ObservableProperty]
	public partial double ListPrice2021conSc { get; set; }

    [ObservableProperty]
	public partial double ListPrice2021conEscnj { get; set; }

    [ObservableProperty]
	public partial double ListPrice2021conEscnj2 { get; set; }

    [ObservableProperty]
	public partial double ListPrice2021sur2 { get; set; }

    [ObservableProperty]
	public partial double ListPrice2021sur2Ny { get; set; }

    [ObservableProperty]
	public partial double ListPrice2021conAepa { get; set; }

    [ObservableProperty]
	public partial double? ListPrice2021conKcda { get; set; }

    [ObservableProperty]
	public partial double ListPrice2021Dec { get; set; }

    [ObservableProperty]
	public partial double ListPrice2022 { get; set; }

    [ObservableProperty]
	public partial double ListPrice2022Nbf { get; set; }

    [ObservableProperty]
	public partial double ListPrice202223conEscnj { get; set; }

    [ObservableProperty]
	public partial double ListPrice2022conBfC { get; set; }

    [ObservableProperty]
	public partial double ListPrice2022conNc { get; set; }

    [ObservableProperty]
	public partial double ListPrice2022conNcepcSsi { get; set; }

    [ObservableProperty]
	public partial double? ListPrice2022conNcepcSse { get; set; }

    [ObservableProperty]
	public partial double ListPrice2022conPeppm { get; set; }

    [ObservableProperty]
	public partial double ListPrice2022conPeppm2 { get; set; }

    [ObservableProperty]
	public partial double ListPrice2022conSc { get; set; }

    [ObservableProperty]
	public partial double ListPrice2022conScA { get; set; }

    [ObservableProperty]
	public partial double ListPrice2022MeTeor { get; set; }

    [ObservableProperty]
	public partial double ListPrice2023 { get; set; }

    [ObservableProperty]
	public partial double ListPrice2023conAepa { get; set; }

    [ObservableProperty]
	public partial double ListPrice2023conEscnj { get; set; }

    [ObservableProperty]
	public partial double ListPrice2023conKcda { get; set; }

    [ObservableProperty]
	public partial double ListPrice2023conIuc { get; set; }

    [ObservableProperty]
	public partial double ListPrice2023conNc { get; set; }

    [ObservableProperty]
	public partial double ListPrice2023conNy { get; set; }

    [ObservableProperty]
	public partial double ListPrice2023conPeppm { get; set; }

    [ObservableProperty]
	public partial double ListPrice2023conSc { get; set; }

    [ObservableProperty]
	public partial double ListPrice2023AtBoces { get; set; }

    [ObservableProperty]
	public partial double ListPrice2023NickersonNy { get; set; }

    [ObservableProperty]
	public partial double ListPrice2023Omnia { get; set; }

    [ObservableProperty]
	public partial double ListPrice2023Sspec { get; set; }

    [ObservableProperty]
	public partial double ListPrice2024 { get; set; }

    [ObservableProperty]
	public partial double ListPrice2024Sspec { get; set; }

    [ObservableProperty]
	public partial bool VircoUscItem { get; set; }

    [ObservableProperty]
	public partial double CurrentUcost { get; set; }

    [ObservableProperty]
	public partial bool Configurable { get; set; }

    [ObservableProperty]
	public partial string? ColorReqd { get; set; }

    [ObservableProperty]
	public partial bool IceEdgePart { get; set; }

    [ObservableProperty]
	public partial bool IceEdge2da { get; set; }

    [ObservableProperty]
	public partial bool IceEdge3da { get; set; }

    [ObservableProperty]
	public partial string? Asset2D { get; set; }

    [ObservableProperty]
	public partial string? Asset3D { get; set; }

    [ObservableProperty]
	public partial string? AssetElv { get; set; }

    [ObservableProperty]
	public partial string? ThreeDlayerName { get; set; }

    [ObservableProperty]
	public partial bool IsPackingListForPreAssembledFrames { get; set; }

    [ObservableProperty]
	public partial bool ShouldShowInManufacturerReport { get; set; }

    [ObservableProperty]
	public partial double WsTedgeUsage { get; set; }

    [ObservableProperty]
	public partial string? Option01 { get; set; }

    [ObservableProperty]
	public partial string? Option02 { get; set; }

    [ObservableProperty]
	public partial string? Option03 { get; set; }

    [ObservableProperty]
	public partial string? Option04 { get; set; }

    [ObservableProperty]
	public partial string? Option05 { get; set; }

    [ObservableProperty]
	public partial string? Option06 { get; set; }

    [ObservableProperty]
	public partial string? Option07 { get; set; }

    [ObservableProperty]
	public partial string? Option08 { get; set; }

    [ObservableProperty]
	public partial string? Option09 { get; set; }

    [ObservableProperty]
	public partial string? Option10 { get; set; }

    [ObservableProperty]
	public partial double HeightOffset { get; set; }

    [ObservableProperty]
	public partial double AcrGrd { get; set; }

    [ObservableProperty]
	public partial double FabGrd { get; set; }

    [ObservableProperty]
	public partial double LamGrd { get; set; }

    [ObservableProperty]
	public partial double MelGrd { get; set; }

    [ObservableProperty]
	public partial double WsLamGrd { get; set; }

    [ObservableProperty]
	public partial string StdPriceGrade { get; set; } = null!;

    [ObservableProperty]
	public partial string? UnspscCode { get; set; }

    [ObservableProperty]
	public partial string? Category01 { get; set; }

    [ObservableProperty]
	public partial string? Category02 { get; set; }

    [ObservableProperty]
	public partial string? Category03 { get; set; }

    [ObservableProperty]
	public partial string? Category04 { get; set; }

    [ObservableProperty]
	public partial string? Category05 { get; set; }

    [ObservableProperty]
	public partial string? Category06 { get; set; }

    [ObservableProperty]
	public partial string? Category07 { get; set; }

    [ObservableProperty]
	public partial string? Category08 { get; set; }

    [ObservableProperty]
	public partial string? Category09 { get; set; }

    [ObservableProperty]
	public partial double QtySoldYtd { get; set; }

    [ObservableProperty]
	public partial bool Manuf { get; set; }

    [ObservableProperty]
	public partial bool SystemsFurn { get; set; }

    [ObservableProperty]
	public virtual partial  ICollection<IcMfgBom> IcMfgBoms { get; set; } = new List<IcMfgBom>();

    [ObservableProperty]
	public virtual partial ICollection<IcProdBom> IcProdBoms { get; set; } = new List<IcProdBom>();
}
