using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public decimal OrderAmount { get; set; }

    public DateTime? OrderDate { get; set; }

    public int CustomerId { get; set; }
    public string Status { get; set; } = "Pending";

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
