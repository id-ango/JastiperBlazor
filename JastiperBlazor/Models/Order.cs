using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JastiperBlazor.Models;

public class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public OrderStatus Status { get; set; } = OrderStatus.PendingDP;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalSellingPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DPAmount { get; set; }

    public DateTime? DPDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal FullPaymentAmount { get; set; }

    public DateTime? FullPaymentDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ShippingCost { get; set; }

    public string? Notes { get; set; }  // Keterangan batch, dll

    public int? SupplierBatchId { get; set; }
    public SupplierBatch? SupplierBatch { get; set; }

    public List<OrderItem> Items { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
