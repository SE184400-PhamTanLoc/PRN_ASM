using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public int CategoryId { get; set; }

    public int? OrderId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Order? Order { get; set; }
}
