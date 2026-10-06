using Day23_MinimalAPI.Data;
using Day23_MinimalAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    Environment.GetEnvironmentVariable(
        "ConnectionStrings__DefaultConnection"
    );

Console.WriteLine($"Connection String: {connectionString}");

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        connectionString
    ));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// GetAll product 
app.MapGet("/api/products", async (AppDbContext db) =>
{
    var products = await db.Products.ToListAsync();

    return Results.Ok(products);
});

// GET BY ID
app.MapGet("/api/products/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);

    if (product == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(product);
});

// POST
app.MapPost("/api/products", async (Product product, AppDbContext db) =>
{
    db.Products.Add(product);

    await db.SaveChangesAsync();

    return Results.Created($"/api/products/{product.Id}", product);
});

// PUT
app.MapPut("/api/products/{id}", async (
    int id,
    Product updatedProduct,
    AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);

    if (product == null)
    {
        return Results.NotFound();
    }

    product.Name = updatedProduct.Name;
    product.Price = updatedProduct.Price;
    product.Quantity = updatedProduct.Quantity;

    await db.SaveChangesAsync();

    return Results.Ok(product);
});

// DELETE
app.MapDelete("/api/products/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);

    if (product == null)
    {
        return Results.NotFound();
    }

    db.Products.Remove(product);

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
