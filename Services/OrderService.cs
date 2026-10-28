using JastiperBlazor.Data;
using JastiperBlazor.Models;
using Microsoft.EntityFrameworkCore;

namespace JastiperBlazor.Services;

public class OrderService
{
    private readonly AppDbContext _db;

    public OrderService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Order>> GetAllAsync()
    {
        return await _db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .Include(o => o.SupplierBatch)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<List<Order>> GetByStatusAsync(OrderStatus status)
    {
        return await _db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
                .ThenInclude(i => i.Supplier)
            .Include(o => o.SupplierBatch)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order> CreateAsync(Order order)
    {
        order.CreatedAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;
        // Hitung total dari items jika ada
        if (order.Items.Any())
        {
            order.TotalSellingPrice = order.Items.Sum(i => i.Quantity * i.UnitSellingPrice);
        }
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return order;
    }

    public async Task UpdateAsync(Order order)
    {
        order.UpdatedAt = DateTime.UtcNow;
        if (order.Items.Any())
        {
            order.TotalSellingPrice = order.Items.Sum(i => i.Quantity * i.UnitSellingPrice);
        }
        _db.Orders.Update(order);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(int orderId, OrderStatus newStatus)
    {
        var order = await _db.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }

    public async Task RecordDPAsync(int orderId, decimal amount, DateTime date)
    {
        var order = await _db.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.DPAmount = amount;
            order.DPDate = date;
            order.Status = OrderStatus.DPReceived;
            order.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }

    public async Task RecordFullPaymentAsync(int orderId, decimal amount, decimal shipping, DateTime date)
    {
        var order = await _db.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.FullPaymentAmount = amount;
            order.ShippingCost = shipping;
            order.FullPaymentDate = date;
            order.Status = OrderStatus.FullPaymentReceived;
            order.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }

    public async Task AddItemAsync(int orderId, OrderItem item)
    {
        item.OrderId = orderId;
        _db.OrderItems.Add(item);
        await _db.SaveChangesAsync();

        // Update total
        var order = await _db.Orders.Include(o => o.Items).FirstAsync(o => o.Id == orderId);
        order.TotalSellingPrice = order.Items.Sum(i => i.Quantity * i.UnitSellingPrice);
        order.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteItemAsync(int itemId)
    {
        var item = await _db.OrderItems.FindAsync(itemId);
        if (item != null)
        {
            var orderId = item.OrderId;
            _db.OrderItems.Remove(item);
            await _db.SaveChangesAsync();

            var order = await _db.Orders.Include(o => o.Items).FirstAsync(o => o.Id == orderId);
            order.TotalSellingPrice = order.Items.Sum(i => i.Quantity * i.UnitSellingPrice);
            order.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }

    public async Task<List<OrderItem>> GetSupplierItemsAsync()
    {
        return await _db.OrderItems
            .Include(i => i.Order)
                .ThenInclude(o => o.Customer)
            .Include(i => i.Order)
                .ThenInclude(o => o.SupplierBatch)
            .Where(i => i.Order.Status == OrderStatus.OrderedToSupplier
                     || i.Order.Status == OrderStatus.PartiallyArrived
                     || i.Order.Status == OrderStatus.Arrived
                     || i.Order.Status == OrderStatus.AwaitingFullPayment
                     || i.Order.Status == OrderStatus.FullPaymentReceived
                     || i.Order.Status == OrderStatus.Shipped
                     || i.Order.Status == OrderStatus.Completed)
            .OrderBy(i => i.ArrivedDate.HasValue)
            .ThenBy(i => i.OrderId)
            .ToListAsync();
    }

    public async Task SetItemArrivalAsync(int itemId, DateTime? arrivedDate)
    {
        var item = await _db.OrderItems
            .Include(i => i.Order)
                .ThenInclude(o => o.Items)
            .FirstOrDefaultAsync(i => i.Id == itemId);
        if (item == null) return;

        item.ArrivedDate = arrivedDate;

        var order = item.Order;
        if (order.Status is OrderStatus.OrderedToSupplier or OrderStatus.PartiallyArrived or OrderStatus.Arrived)
        {
            var arrivedCount = order.Items.Count(i => i.ArrivedDate.HasValue);
            order.Status = arrivedCount == 0
                ? OrderStatus.OrderedToSupplier
                : arrivedCount == order.Items.Count
                    ? OrderStatus.Arrived
                    : OrderStatus.PartiallyArrived;
        }

        order.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}
