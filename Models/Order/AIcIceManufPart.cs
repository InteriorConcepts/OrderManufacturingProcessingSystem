using System;
using System.Collections.Generic;

namespace OMPS.Models.Order;

public partial class AIcIceManufPart
{
    public Guid IceManufPartId { get; set; }

    public Guid? ColorSetId { get; set; }

    public Guid? IceManufId { get; set; }

    public string PartNbr { get; set; } = null!;

    public string? PartDescription { get; set; }

    public double Qty { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime ChangeDate { get; set; }

    public Guid ChangedbyId { get; set; }
}
