using System.ComponentModel.DataAnnotations;

namespace JastiperBlazor.Models;

public class Customer
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string WhatsAppNumber { get; set; } = string.Empty;

    public string? Address { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Order> Orders { get; set; } = new();
}
