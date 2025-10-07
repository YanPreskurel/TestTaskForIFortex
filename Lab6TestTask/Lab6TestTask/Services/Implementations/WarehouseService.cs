using Lab6TestTask.Data;
using Lab6TestTask.Enums;
using Lab6TestTask.Models;
using Lab6TestTask.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lab6TestTask.Services.Implementations;

/// <summary>
/// WarehouseService.
/// Implement methods here.
/// </summary>
public class WarehouseService : IWarehouseService
{
    private readonly ApplicationDbContext _dbContext;

    public WarehouseService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Warehouse> GetWarehouseAsync()
    {
        if (!await _dbContext.Warehouses.AnyAsync())
            throw new InvalidOperationException("Таблица Warehouses пуста.");

        if (!await _dbContext.Products.AnyAsync())
            throw new InvalidOperationException("Таблица Products пуста.");

        var warehouse = await _dbContext.Warehouses
            .Include(w => w.Products)
            .OrderByDescending(w => w.Products
                .Where(p => p.Status == ProductStatus.ReadyForDistribution)
                .Sum(p => p.Price * p.Quantity))
            .FirstOrDefaultAsync();

        if (warehouse == null)
            throw new InvalidOperationException("Не найдено складов с готовыми к распределению продуктами.");

        return warehouse;
    }

    public async Task<IEnumerable<Warehouse>> GetWarehousesAsync()
    {
        if (!await _dbContext.Warehouses.AnyAsync())
            throw new InvalidOperationException("Таблица Warehouses пуста.");

        if (!await _dbContext.Products.AnyAsync())
            throw new InvalidOperationException("Таблица Products пуста.");

        var warehouses = await _dbContext.Warehouses
            .Include(w => w.Products)
            .Where(w => w.Products.Any(p =>
                p.ReceivedDate.Year == 2025 &&
                p.ReceivedDate.Month >= 4 &&
                p.ReceivedDate.Month <= 6))
            .ToListAsync();

        if (!warehouses.Any())
            throw new InvalidOperationException("Нет складов, получивших товары во втором квартале 2025 года.");

        return warehouses;
    }
}
