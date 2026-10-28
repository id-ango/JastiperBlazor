using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JastiperBlazor.Models;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    [Required]
    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; } = 1;

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitSellingPrice { get; set; }

    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public string? SupplierCountry { get; set; }  // Korea / China

    [Column(TypeName = "decimal(18,2)")]
    public decimal? SupplierCost { get; set; }  // Biaya ke supplier (opsional, untuk margin)

    public string? Notes { get; set; }

    public DateTime? ArrivedDate { get; set; }  // Tanggal barang sampai di Indonesia (null = belum sampai)
}
