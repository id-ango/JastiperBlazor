using JastiperBlazor.Data;
using JastiperBlazor.Models;
using Microsoft.EntityFrameworkCore;

namespace JastiperBlazor.Services;

public class SupplierService
{
    private readonly AppDbContext _db;

    public SupplierService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Supplier>> SearchAsync(string term)
    {
        var key = term.ToLower();
        return await _db.Suppliers
            .Where(s => s.Name.ToLower().Contains(key) || (s.Country != null && s.Country.ToLower().Contains(key)))
            .OrderBy(s => s.Name)
            .Take(20)
            .ToListAsync();
    }

    public async Task<Supplier> CreateAsync(Supplier supplier)
    {
        supplier.CreatedAt = DateTime.UtcNow;
        _db.Suppliers.Add(supplier);
        await _db.SaveChangesAsync();
        return supplier;
    }
}

