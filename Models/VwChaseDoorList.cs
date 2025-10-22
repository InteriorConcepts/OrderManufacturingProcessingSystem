using System;
using System.Collections.Generic;

namespace OMPS.Models;

public partial class VwChaseDoorList
{
    public Guid ItemId { get; set; }

    public string Item { get; set; } = null!;

    public string? Description { get; set; }

    public string? Status { get; set; }

    public string? ProductCategory { get; set; }

    public string? ProductSubCategory { get; set; }
}
