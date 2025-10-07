using Lab6TestTask.Data;
using Lab6TestTask.Enums;
using Lab6TestTask.Models;
using Lab6TestTask.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lab6TestTask.Services.Implementations;

/// <summary>
/// ProductService.
/// Implement methods here.
/// </summary>
public class ProductService : IProductService
{
    private readonly ApplicationDbContext _dbContext;

    public ProductService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Product> GetProductAsync()
    {
        if (!await _dbContext.Products.AnyAsync())
            throw new InvalidOperationException("Таблица Products пуста.");

        var product = await _dbContext.Products
            .Where(p => p.Status == ProductStatus.Reserved)
            .OrderByDescending(p => p.Price)
            .FirstOrDefaultAsync();

        if (product == null)
            throw new InvalidOperationException("Нет зарезервированных продуктов в базе данных.");

        return product;
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        if (!await _dbContext.Products.AnyAsync())
            throw new InvalidOperationException("Таблица Products пуста.");

        var products = await _dbContext.Products
            .Where(p => p.ReceivedDate.Year == 2025 && p.Quantity > 1000)
            .ToListAsync();

        if (!products.Any())
            throw new InvalidOperationException("Нет продуктов, доставленных в 2025 году с количеством больше 1000.");

        return products;
    }
}
