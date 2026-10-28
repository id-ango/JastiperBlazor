using System.ComponentModel.DataAnnotations;

namespace JastiperBlazor.Models;

public class Supplier
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Country { get; set; }  // Korea / China / Lainnya

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

