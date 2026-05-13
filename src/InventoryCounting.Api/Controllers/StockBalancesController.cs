using InventoryCounting.Application.DTOs.StockBalances;
using InventoryCounting.Domain.Entities;
using InventoryCounting.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryCounting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockBalancesController : ControllerBase
{
    private readonly AppDbContext _context;

    public StockBalancesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockBalanceResponse>>> GetStockBalances()
    {
        var stockBalances = await _context.StockBalances
            .Include(stockBalance => stockBalance.Product)
            .Include(stockBalance => stockBalance.Warehouse)
            .OrderBy(stockBalance => stockBalance.Warehouse.Name)
            .ThenBy(stockBalance => stockBalance.Product.Code)
            .Select(stockBalance => new StockBalanceResponse
            {
                Id = stockBalance.Id,
                ProductId = stockBalance.ProductId,
                ProductCode = stockBalance.Product.Code,
                ProductName = stockBalance.Product.Name,
                WarehouseId = stockBalance.WarehouseId,
                WarehouseName = stockBalance.Warehouse.Name,
                Quantity = stockBalance.Quantity,
                LastUpdatedAt = stockBalance.LastUpdatedAt
            })
            .ToListAsync();

        return Ok(stockBalances);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StockBalanceResponse>> GetStockBalance(int id)
    {
        var stockBalance = await _context.StockBalances
            .Include(currentStockBalance => currentStockBalance.Product)
            .Include(currentStockBalance => currentStockBalance.Warehouse)
            .Select(currentStockBalance => new StockBalanceResponse
            {
                Id = currentStockBalance.Id,
                ProductId = currentStockBalance.ProductId,
                ProductCode = currentStockBalance.Product.Code,
                ProductName = currentStockBalance.Product.Name,
                WarehouseId = currentStockBalance.WarehouseId,
                WarehouseName = currentStockBalance.Warehouse.Name,
                Quantity = currentStockBalance.Quantity,
                LastUpdatedAt = currentStockBalance.LastUpdatedAt
            })
            .FirstOrDefaultAsync(currentStockBalance => currentStockBalance.Id == id);

        if (stockBalance is null)
        {
            return NotFound();
        }

        return Ok(stockBalance);
    }

    [HttpGet("warehouse/{warehouseId:int}")]
    public async Task<ActionResult<IEnumerable<StockBalanceResponse>>> GetStockBalancesByWarehouse(int warehouseId)
    {
        var warehouseExists = await _context.Warehouses
            .AnyAsync(warehouse => warehouse.Id == warehouseId && warehouse.IsActive);

        if (!warehouseExists)
        {
            return NotFound();
        }

        var stockBalances = await _context.StockBalances
            .Include(stockBalance => stockBalance.Product)
            .Include(stockBalance => stockBalance.Warehouse)
            .Where(stockBalance => stockBalance.WarehouseId == warehouseId)
            .OrderBy(stockBalance => stockBalance.Product.Code)
            .Select(stockBalance => new StockBalanceResponse
            {
                Id = stockBalance.Id,
                ProductId = stockBalance.ProductId,
                ProductCode = stockBalance.Product.Code,
                ProductName = stockBalance.Product.Name,
                WarehouseId = stockBalance.WarehouseId,
                WarehouseName = stockBalance.Warehouse.Name,
                Quantity = stockBalance.Quantity,
                LastUpdatedAt = stockBalance.LastUpdatedAt
            })
            .ToListAsync();

        return Ok(stockBalances);
    }

    [HttpPost]
    public async Task<ActionResult<StockBalanceResponse>> CreateStockBalance(CreateStockBalanceRequest request)
    {
        var productExists = await _context.Products
            .AnyAsync(product => product.Id == request.ProductId && product.IsActive);

        if (!productExists)
        {
            ModelState.AddModelError(nameof(request.ProductId), "Product must exist and be active.");
        }

        var warehouseExists = await _context.Warehouses
            .AnyAsync(warehouse => warehouse.Id == request.WarehouseId && warehouse.IsActive);

        if (!warehouseExists)
        {
            ModelState.AddModelError(nameof(request.WarehouseId), "Warehouse must exist and be active.");
        }

        if (request.Quantity < 0)
        {
            ModelState.AddModelError(nameof(request.Quantity), "Quantity cannot be negative.");
        }

        var duplicateExists = await _context.StockBalances
            .AnyAsync(existingStockBalance =>
                existingStockBalance.ProductId == request.ProductId
                && existingStockBalance.WarehouseId == request.WarehouseId);

        if (duplicateExists)
        {
            ModelState.AddModelError(
                nameof(request.ProductId),
                "Only one stock balance is allowed per product and warehouse.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var newStockBalance = new StockBalance
        {
            ProductId = request.ProductId,
            WarehouseId = request.WarehouseId,
            Quantity = request.Quantity,
            LastUpdatedAt = DateTime.UtcNow
        };

        _context.StockBalances.Add(newStockBalance);
        await _context.SaveChangesAsync();

        var createdStockBalance = await _context.StockBalances
            .Include(stockBalance => stockBalance.Product)
            .Include(stockBalance => stockBalance.Warehouse)
            .Where(stockBalance => stockBalance.Id == newStockBalance.Id)
            .Select(stockBalance => new StockBalanceResponse
            {
                Id = stockBalance.Id,
                ProductId = stockBalance.ProductId,
                ProductCode = stockBalance.Product.Code,
                ProductName = stockBalance.Product.Name,
                WarehouseId = stockBalance.WarehouseId,
                WarehouseName = stockBalance.Warehouse.Name,
                Quantity = stockBalance.Quantity,
                LastUpdatedAt = stockBalance.LastUpdatedAt
            })
            .FirstAsync();

        return CreatedAtAction(nameof(GetStockBalance), new { id = createdStockBalance.Id }, createdStockBalance);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateStockBalance(int id, UpdateStockBalanceRequest request)
    {
        var existingStockBalance = await _context.StockBalances
            .FirstOrDefaultAsync(currentStockBalance => currentStockBalance.Id == id);

        if (existingStockBalance is null)
        {
            return NotFound();
        }

        if (request.Quantity < 0)
        {
            ModelState.AddModelError(nameof(request.Quantity), "Quantity cannot be negative.");
            return ValidationProblem(ModelState);
        }

        existingStockBalance.Quantity = request.Quantity;
        existingStockBalance.LastUpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
