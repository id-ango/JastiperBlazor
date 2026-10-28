using JastiperBlazor.Data;
using Microsoft.EntityFrameworkCore;

namespace JastiperBlazor.Services;

public class ProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<string>> GetAllProductNamesAsync()
    {
        return await _db.OrderItems
            .Select(i => i.ProductName)
            .Distinct()
            .OrderBy(n => n)
            .ToListAsync();
    }

    public async Task<List<string>> SearchProductNamesAsync(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return await GetAllProductNamesAsync();

        return await _db.OrderItems
            .Where(i => i.ProductName.Contains(term))
            .Select(i => i.ProductName)
            .Distinct()
            .OrderBy(n => n)
            .Take(20)
            .ToListAsync();
    }

    public async Task RenameProductAsync(string oldName, string newName)
    {
        if (string.IsNullOrWhiteSpace(oldName) || string.IsNullOrWhiteSpace(newName) || oldName == newName)
            return;

        var items = await _db.OrderItems
            .Where(i => i.ProductName == oldName)
            .ToListAsync();

        foreach (var item in items)
        {
            item.ProductName = newName;
        }

        await _db.SaveChangesAsync();
    }

    public async Task<int> GetUsageCountAsync(string productName)
    {
        return await _db.OrderItems.CountAsync(i => i.ProductName == productName);
    }
}
