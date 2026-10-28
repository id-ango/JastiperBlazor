using JastiperBlazor.Data;
using JastiperBlazor.Models;
using Microsoft.EntityFrameworkCore;

namespace JastiperBlazor.Services;

public class CustomerService
{
    private readonly AppDbContext _db;

    public CustomerService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        return await _db.Customers
            .Include(c => c.Orders)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _db.Customers
            .Include(c => c.Orders)
                .ThenInclude(o => o.Items)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Customer> CreateAsync(Customer customer)
    {
        customer.CreatedAt = DateTime.UtcNow;
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
        return customer;
    }

    public async Task UpdateAsync(Customer customer)
    {
        _db.Customers.Update(customer);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var customer = await _db.Customers.FindAsync(id);
        if (customer != null)
        {
            _db.Customers.Remove(customer);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<List<Customer>> SearchAsync(string term)
    {
        var key = term.ToLower();
        return await _db.Customers
            .Where(c => c.Name.ToLower().Contains(key) || c.WhatsAppNumber.ToLower().Contains(key))
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}
