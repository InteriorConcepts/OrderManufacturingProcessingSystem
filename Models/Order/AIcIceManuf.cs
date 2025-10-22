using System;
using System.Collections.Generic;

namespace OMPS.Models.Order;

public partial class AIcIceManuf
{
    public Guid IceManufId { get; set; }

    public Guid? ColorSetId { get; set; }

    public Guid? ProductId { get; set; }

    public Guid? ProductLinkId { get; set; }

    public Guid? ItemId { get; set; }

    public string? QuoteNbr { get; set; }

    public string? JobNbr { get; set; }

    public string? CustOrderNbr { get; set; }

    public string? PartNbr { get; set; }

    public string? ItemNbr { get; set; }

    public string? Idnbr { get; set; }

    public string? CatalogNbr { get; set; }

    public double? Qty { get; set; }

    public string? Type { get; set; }

    public string? SubType { get; set; }

    public string? Description { get; set; }

    public string? UofM { get; set; }

    public string? ItemFin { get; set; }

    public string? ItemCore { get; set; }

    public string? ColorBy { get; set; }

    public string? Dept { get; set; }

    public string? WorkCtr { get; set; }

    public double? ScrapFactor { get; set; }

    public double? SizeDivisor { get; set; }

    public double? Depth { get; set; }

    public double? Width { get; set; }

    public double? Fabwidth { get; set; }

    public double? Height { get; set; }

    public double? FabHeight { get; set; }

    public string? Assembled { get; set; }

    public string? AssyNbr { get; set; }

    public string? TileIndicator { get; set; }

    public bool? Explode { get; set; }

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

    public string? Usertag1 { get; set; }

    public string? CoreSize { get; set; }

    public double Multiplier { get; set; }

    public string? Area { get; set; }

    public Guid CreatedById { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime ChangeDate { get; set; }

    public Guid ChangedbyId { get; set; }
}
