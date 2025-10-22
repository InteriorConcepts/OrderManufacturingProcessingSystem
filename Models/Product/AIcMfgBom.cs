using System;
using System.Collections.Generic;

namespace OMPS.Models.Product;

public partial class AIcMfgBom
{
    public Guid MfgBomId { get; set; }

    public Guid? ProductId { get; set; }

    public Guid? ItemId { get; set; }

    public string? ItemNbr { get; set; }

    public string? Description { get; set; }

    public string Type { get; set; } = null!;

    public double? Quantity { get; set; }

    public string UofM { get; set; } = null!;

    public double Cost { get; set; }

    public string? Dept { get; set; }

    public string? WorkCtr { get; set; }

    public string? Assembled { get; set; }

    public double Weight { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime ChangeDate { get; set; }

    public Guid ChangedById { get; set; }

    public virtual IcProductCatalog? Product { get; set; }
}
