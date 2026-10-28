using System.ComponentModel.DataAnnotations.Schema;

namespace JastiperBlazor.Models;

public class SupplierBatch
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;  // e.g. "Batch Korea Oktober 2026"

    public string? SupplierCountry { get; set; }  // Korea / China

    public DateTime OrderDateToSupplier { get; set; } = DateTime.UtcNow;

    public DateTime? PaymentDateToSupplier { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalCostToSupplier { get; set; }

    public string? Notes { get; set; }

    public List<Order> Orders { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
