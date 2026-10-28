using JastiperBlazor.Data;
using JastiperBlazor.Models;
using Microsoft.EntityFrameworkCore;

namespace JastiperBlazor.Services;

public class ReportService
{
    private readonly AppDbContext _db;

    public ReportService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<TopItemDto>> GetTopSellingItemsAsync(int top = 10)
    {
        return await _db.OrderItems
            .GroupBy(i => i.ProductName)
            .Select(g => new TopItemDto
            {
                ProductName = g.Key,
                TotalQuantity = g.Sum(i => i.Quantity),
                TotalRevenue = g.Sum(i => i.Quantity * i.UnitSellingPrice)
            })
            .OrderByDescending(x => x.TotalQuantity)
            .Take(top)
            .ToListAsync();
    }

    public async Task<List<TopCustomerDto>> GetTopCustomersAsync(int top = 10)
    {
        return await _db.Orders
            .Where(o => o.Status != OrderStatus.Cancelled)
            .GroupBy(o => o.Customer)
            .Select(g => new TopCustomerDto
            {
                Customer = g.Key,
                OrderCount = g.Count(),
                TotalSpent = g.Sum(o => o.TotalSellingPrice + o.ShippingCost)
            })
            .OrderByDescending(x => x.TotalSpent)
            .Take(top)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime? from = null, DateTime? to = null)
    {
        var query = _db.Orders.Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Shipped || o.Status == OrderStatus.FullPaymentReceived);

        if (from.HasValue)
            query = query.Where(o => o.OrderDate >= from.Value);
        if (to.HasValue)
            query = query.Where(o => o.OrderDate <= to.Value);

        return await query.SumAsync(o => o.TotalSellingPrice + o.ShippingCost);
    }

    public async Task<Dictionary<OrderStatus, int>> GetOrderStatusSummaryAsync()
    {
        return await _db.Orders
            .GroupBy(o => o.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count);
    }
}
