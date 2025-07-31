namespace BusinessLayer.DTO;

public class OrderDTO
{
    public int OrderId { get; set; }
    public decimal OrderAmount { get; set; }
    public DateTime? OrderDate { get; set; }
    public int CustomerId { get; set; }
    public string Status { get; set; } = "Pending";
    public List<int> ProductIds { get; set; } = new List<int>();
    
    // Additional properties for admin display
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public int ProductCount { get; set; }
    public string FormattedOrderDate => OrderDate?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
    public string FormattedAmount => $"${OrderAmount:N2}";
}
