using JastiperBlazor.Data;
using JastiperBlazor.Models;
using JastiperBlazor.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=jastiper.db"));

// Services
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<SupplierBatchService>();
builder.Services.AddScoped<SupplierService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<ProductService>();

var app = builder.Build();

// Ensure database is created and seed sample data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS Suppliers (Id INTEGER NOT NULL CONSTRAINT PK_Suppliers PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL, Country TEXT NULL, Notes TEXT NULL, CreatedAt TEXT NOT NULL)");

    var orderItemColumns = db.Database.SqlQueryRaw<string>("SELECT name FROM pragma_table_info('OrderItems')").ToList();
    if (!orderItemColumns.Contains("ArrivedDate"))
    {
        db.Database.ExecuteSqlRaw("ALTER TABLE OrderItems ADD COLUMN ArrivedDate TEXT NULL");
    }
    if (!orderItemColumns.Contains("SupplierId"))
    {
        db.Database.ExecuteSqlRaw("ALTER TABLE OrderItems ADD COLUMN SupplierId INTEGER NULL REFERENCES Suppliers(Id) ON DELETE SET NULL");
    }

    if (!db.Customers.Any())
    {
        var c1 = new Customer { Name = "Siti Aminah", WhatsAppNumber = "081234567890", Address = "Jakarta Selatan", Notes = "Sering beli skincare" };
        var c2 = new Customer { Name = "Budi Santoso", WhatsAppNumber = "081987654321", Address = "Bandung", Notes = "Suka fashion Korea" };
        db.Customers.AddRange(c1, c2);
        db.SaveChanges();

        var o1 = new Order
        {
            CustomerId = c1.Id,
            OrderDate = DateTime.UtcNow.AddDays(-5),
            Status = OrderStatus.DPReceived,
            DPAmount = 150000,
            DPDate = DateTime.UtcNow.AddDays(-4),
            Notes = "Batch Korea Oktober",
            Items = new List<OrderItem>
            {
                new() { ProductName = "Serum Some By Mi", Quantity = 1, UnitSellingPrice = 250000, SupplierCountry = "Korea" },
                new() { ProductName = "Toner Round Lab", Quantity = 1, UnitSellingPrice = 180000, SupplierCountry = "Korea" }
            }
        };
        o1.TotalSellingPrice = o1.Items.Sum(i => i.Quantity * i.UnitSellingPrice);

        db.Orders.Add(o1);
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
