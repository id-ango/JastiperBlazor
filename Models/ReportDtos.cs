namespace JastiperBlazor.Models;

public class TopItemDto
{
    public string ProductName { get; set; } = string.Empty;
    public int TotalQuantity { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class TopCustomerDto
{
    public Customer Customer { get; set; } = null!;
    public int OrderCount { get; set; }
    public decimal TotalSpent { get; set; }
}
