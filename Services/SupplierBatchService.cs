using JastiperBlazor.Data;
using JastiperBlazor.Models;
using Microsoft.EntityFrameworkCore;

namespace JastiperBlazor.Services;

public class SupplierBatchService
{
    private readonly AppDbContext _db;

    public SupplierBatchService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<SupplierBatch>> GetAllAsync()
    {
        return await _db.SupplierBatches
            .Include(b => b.Orders)
                .ThenInclude(o => o.Customer)
            .OrderByDescending(b => b.OrderDateToSupplier)
            .ToListAsync();
    }

    public async Task<SupplierBatch?> GetByIdAsync(int id)
    {
        return await _db.SupplierBatches
            .Include(b => b.Orders)
                .ThenInclude(o => o.Items)
            .Include(b => b.Orders)
                .ThenInclude(o => o.Customer)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<SupplierBatch> CreateAsync(SupplierBatch batch)
    {
        batch.CreatedAt = DateTime.UtcNow;
        _db.SupplierBatches.Add(batch);
        await _db.SaveChangesAsync();
        return batch;
    }

    public async Task UpdateAsync(SupplierBatch batch)
    {
        _db.SupplierBatches.Update(batch);
        await _db.SaveChangesAsync();
    }

    public async Task AssignOrdersAsync(int batchId, List<int> orderIds)
    {
        var batch = await _db.SupplierBatches.FindAsync(batchId);
        if (batch == null) return;

        var orders = await _db.Orders
            .Where(o => orderIds.Contains(o.Id) && o.Status == OrderStatus.DPReceived)
            .ToListAsync();

        foreach (var order in orders)
        {
            order.SupplierBatchId = batchId;
            order.Status = OrderStatus.OrderedToSupplier;
            order.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
    }

    public async Task MarkPaymentToSupplierAsync(int batchId, decimal totalCost, DateTime paymentDate)
    {
        var batch = await _db.SupplierBatches.FindAsync(batchId);
        if (batch != null)
        {
            batch.TotalCostToSupplier = totalCost;
            batch.PaymentDateToSupplier = paymentDate;
            await _db.SaveChangesAsync();
        }
    }
}
