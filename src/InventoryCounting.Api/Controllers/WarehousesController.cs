using InventoryCounting.Domain.Entities;
using InventoryCounting.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryCounting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehousesController : ControllerBase
{
    private readonly AppDbContext _context;

    public WarehousesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Warehouse>>> GetWarehouses()
    {
        var warehouses = await _context.Warehouses
            .Where(warehouse => warehouse.IsActive)
            .OrderBy(warehouse => warehouse.Name)
            .ToListAsync();

        return Ok(warehouses);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Warehouse>> GetWarehouse(int id)
    {
        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(warehouse => warehouse.Id == id && warehouse.IsActive);

        if (warehouse is null)
        {
            return NotFound();
        }

        return Ok(warehouse);
    }

    [HttpPost]
    public async Task<ActionResult<Warehouse>> CreateWarehouse(Warehouse warehouse)
    {
        if (string.IsNullOrWhiteSpace(warehouse.Name))
        {
            ModelState.AddModelError(nameof(warehouse.Name), "Name is required.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        warehouse.CreatedAt = DateTime.UtcNow;
        warehouse.IsActive = true;

        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetWarehouse), new { id = warehouse.Id }, warehouse);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateWarehouse(int id, Warehouse warehouse)
    {
        var existingWarehouse = await _context.Warehouses
            .FirstOrDefaultAsync(currentWarehouse => currentWarehouse.Id == id && currentWarehouse.IsActive);

        if (existingWarehouse is null)
        {
            return NotFound();
        }

        existingWarehouse.Name = warehouse.Name;
        existingWarehouse.Location = warehouse.Location;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteWarehouse(int id)
    {
        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(currentWarehouse => currentWarehouse.Id == id && currentWarehouse.IsActive);

        if (warehouse is null)
        {
            return NotFound();
        }

        warehouse.IsActive = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
