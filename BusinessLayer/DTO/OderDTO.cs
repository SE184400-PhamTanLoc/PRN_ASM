namespace BusinessLayer.DTOs;

public class OrderDTO
{
    public int OrderId { get; set; }
    public decimal OrderAmount { get; set; }
    public DateTime? OrderDate { get; set; }
    public int CustomerId { get; set; }
    public string Status { get; set; } = "Pending";
    public List<int> ProductIds { get; set; } = new List<int>(); 
}
