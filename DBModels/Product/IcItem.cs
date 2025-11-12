using System;
using System.Collections.Generic;

namespace OMPS.DBModels.Product;

public partial class IcItem
{
    public Guid ItemId { get; set; }

    public string Item { get; set; } = null!;

    public string? Description { get; set; }

    public string? UofM { get; set; }

    public string? ItemFin { get; set; }

    public string? CatalogNbr { get; set; }

    public string? ColorBy { get; set; }

    public string? WorkCtr { get; set; }

    public decimal? ScrapFactor { get; set; }

    public decimal? SizeDivisor { get; set; }

    public decimal? ItemDepth { get; set; }

    public decimal? ItemWidth { get; set; }

    public decimal? ItemHeight { get; set; }

    public decimal? ItemWeight { get; set; }

    public string? ProductCategory { get; set; }

    public string? ProductSubCategory { get; set; }

    public double? ListPrice { get; set; }

    public decimal? ColorPosInName { get; set; }

    public string? PartLocation { get; set; }

    public decimal? PriceCategory { get; set; }

    public string? Directional { get; set; }

    public decimal? ProductCatalog { get; set; }

    public DateTime? RecordDate { get; set; }

    public string? Status { get; set; }

    public DateTime? CreationDate { get; set; }

    public Guid? ChangedById { get; set; }

    public DateTime? ChangeDate { get; set; }
}
