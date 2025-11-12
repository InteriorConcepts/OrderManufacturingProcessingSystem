using System;
using System.Collections.Generic;

namespace OMPS.DBModels.Product;

public partial class AIcProdBom
{
    public Guid AIcProdBomId { get; set; }

    public Guid ProductId { get; set; }

    public Guid LinkId { get; set; }

    public double Quantity { get; set; }

    public string? ProductCode { get; set; }

    public string? Description { get; set; }

    public string UofM { get; set; } = null!;

    public string? Type { get; set; }

    public string? SubType { get; set; }

    public string? Dept { get; set; }

    public double? QuotedListPrice { get; set; }

    public double? StandardPrice { get; set; }

    public double? StdNetPrice { get; set; }

    public double? Cost { get; set; }

    public double Depth { get; set; }

    public double Width { get; set; }

    public double Length { get; set; }

    public double Height { get; set; }

    public double? Weight { get; set; }

    public string? Status { get; set; }

    public string? ColorBy { get; set; }

    public string? Color { get; set; }

    public string? Assembled { get; set; }

    public string? WorkCtr { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime ChangeDate { get; set; }

    public Guid ChangedById { get; set; }

    public virtual IcProductCatalog Product { get; set; } = null!;
}
