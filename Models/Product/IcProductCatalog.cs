using System;
using System.Collections.Generic;

namespace OMPS.Models.Product;

public partial class IcProductCatalog
{
    public Guid ProductId { get; set; }

    public Guid SupplierId { get; set; }

    public Guid? CategoryId { get; set; }

    public Guid? ContractId { get; set; }

    public Guid? QuoteId { get; set; }

    public Guid? QuotePartId { get; set; }

    public string? QuoteNbr { get; set; }

    public string? QuoteItemNbr { get; set; }

    public string? QuotePtNbr { get; set; }

    public string? QuoteRev { get; set; }

    public string? CatalogPage { get; set; }

    public string ProductCode { get; set; } = null!;

    public string? Description { get; set; }

    public string? Description2 { get; set; }

    public string UofM { get; set; } = null!;

    public string? Type { get; set; }

    public string? SubType { get; set; }

    public bool FurnCatalog { get; set; }

    public bool ProjectMatrix { get; set; }

    public bool Cet { get; set; }

    public bool ExpressLane { get; set; }

    public bool ExpressLaneComplete { get; set; }

    public string? DwgItemNbr { get; set; }

    public double StandardPrice { get; set; }

    public double Cost { get; set; }

    public DateTime? CostRecordDate { get; set; }

    public Guid CreatedById { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime ChangeDate { get; set; }

    public double StdNetPrice { get; set; }

    public double Weight { get; set; }

    public double Width { get; set; }

    public double Depth { get; set; }

    public double Height { get; set; }

    public string Status { get; set; } = null!;

    public string? StatusOld { get; set; }

    public string? ColorBy { get; set; }

    public string? Color { get; set; }

    public string? Finish { get; set; }

    public bool Explode { get; set; }

    public bool ExplodePre { get; set; }

    public bool PreList { get; set; }

    public string? PricingComment { get; set; }

    public double CostRollup { get; set; }

    public double ListPriceRollup { get; set; }

    public double WeightRollup { get; set; }

    public bool RollupPrice { get; set; }

    public double CurrentList { get; set; }

    public double PreviousList { get; set; }

    public bool Contract { get; set; }

    public double ListPrice2010 { get; set; }

    public double ListPrice2011 { get; set; }

    public double ListPrice2012 { get; set; }

    public double ListPrice2013 { get; set; }

    public double ListPrice2014 { get; set; }

    public double ListPrice2015 { get; set; }

    public double ListPrice2016 { get; set; }

    public double ListPrice2017 { get; set; }

    public double ListPrice2017usc { get; set; }

    public double ListPrice2018 { get; set; }

    public double ListPrice2018con { get; set; }

    public double ListPrice2018t { get; set; }

    public double ListPrice2018Iccdic { get; set; }

    public double ListPrice2019 { get; set; }

    public double ListPrice2019con { get; set; }

    public double? ListPrice2019t { get; set; }

    public double ListPrice2020 { get; set; }

    public double ListPrice2020Nj { get; set; }

    public double ListPrice2020Con { get; set; }

    public double ListPrice2020ConA { get; set; }

    public double ListPrice2020ConMs { get; set; }

    public double ListPrice2020conMapt { get; set; }

    public double ListPrice2020con10Esc { get; set; }

    public double ListPrice2020Temp { get; set; }

    public double ListPrice2021 { get; set; }

    public double ListPrice2021conDpd { get; set; }

    public double ListPrice2021conMs { get; set; }

    public double ListPrice2021conNc { get; set; }

    public double ListPrice2021conNynj { get; set; }

    public double ListPrice2021conPa { get; set; }

    public double ListPrice2021conPeppm { get; set; }

    public double ListPrice2021conSc { get; set; }

    public double ListPrice2021conEscnj { get; set; }

    public double ListPrice2021conEscnj2 { get; set; }

    public double ListPrice2021sur2 { get; set; }

    public double ListPrice2021sur2Ny { get; set; }

    public double ListPrice2021conAepa { get; set; }

    public double? ListPrice2021conKcda { get; set; }

    public double ListPrice2021Dec { get; set; }

    public double ListPrice2022 { get; set; }

    public double ListPrice2022Nbf { get; set; }

    public double ListPrice202223conEscnj { get; set; }

    public double ListPrice2022conBfC { get; set; }

    public double ListPrice2022conNc { get; set; }

    public double ListPrice2022conNcepcSsi { get; set; }

    public double? ListPrice2022conNcepcSse { get; set; }

    public double ListPrice2022conPeppm { get; set; }

    public double ListPrice2022conPeppm2 { get; set; }

    public double ListPrice2022conSc { get; set; }

    public double ListPrice2022conScA { get; set; }

    public double ListPrice2022MeTeor { get; set; }

    public double ListPrice2023 { get; set; }

    public double ListPrice2023conAepa { get; set; }

    public double ListPrice2023conEscnj { get; set; }

    public double ListPrice2023conKcda { get; set; }

    public double ListPrice2023conIuc { get; set; }

    public double ListPrice2023conNc { get; set; }

    public double ListPrice2023conNy { get; set; }

    public double ListPrice2023conPeppm { get; set; }

    public double ListPrice2023conSc { get; set; }

    public double ListPrice2023AtBoces { get; set; }

    public double ListPrice2023NickersonNy { get; set; }

    public double ListPrice2023Omnia { get; set; }

    public double ListPrice2023Sspec { get; set; }

    public double ListPrice2024 { get; set; }

    public double ListPrice2024Sspec { get; set; }

    public bool VircoUscItem { get; set; }

    public double CurrentUcost { get; set; }

    public bool Configurable { get; set; }

    public string? ColorReqd { get; set; }

    public bool IceEdgePart { get; set; }

    public bool IceEdge2da { get; set; }

    public bool IceEdge3da { get; set; }

    public string? Asset2D { get; set; }

    public string? Asset3D { get; set; }

    public string? AssetElv { get; set; }

    public string? ThreeDlayerName { get; set; }

    public bool IsPackingListForPreAssembledFrames { get; set; }

    public bool ShouldShowInManufacturerReport { get; set; }

    public double WsTedgeUsage { get; set; }

    public string? Option01 { get; set; }

    public string? Option02 { get; set; }

    public string? Option03 { get; set; }

    public string? Option04 { get; set; }

    public string? Option05 { get; set; }

    public string? Option06 { get; set; }

    public string? Option07 { get; set; }

    public string? Option08 { get; set; }

    public string? Option09 { get; set; }

    public string? Option10 { get; set; }

    public double HeightOffset { get; set; }

    public double AcrGrd { get; set; }

    public double FabGrd { get; set; }

    public double LamGrd { get; set; }

    public double MelGrd { get; set; }

    public double WsLamGrd { get; set; }

    public string StdPriceGrade { get; set; } = null!;

    public string? UnspscCode { get; set; }

    public string? Category01 { get; set; }

    public string? Category02 { get; set; }

    public string? Category03 { get; set; }

    public string? Category04 { get; set; }

    public string? Category05 { get; set; }

    public string? Category06 { get; set; }

    public string? Category07 { get; set; }

    public string? Category08 { get; set; }

    public string? Category09 { get; set; }

    public double QtySoldYtd { get; set; }

    public bool Manuf { get; set; }

    public bool SystemsFurn { get; set; }

    public virtual ICollection<AIcMfgBom> AIcMfgBoms { get; set; } = new List<AIcMfgBom>();

    public virtual ICollection<AIcProdBom> AIcProdBoms { get; set; } = new List<AIcProdBom>();
}
