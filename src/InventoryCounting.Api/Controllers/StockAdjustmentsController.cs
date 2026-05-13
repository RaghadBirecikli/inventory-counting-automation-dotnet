using InventoryCounting.Application.DTOs.StockAdjustments;
using InventoryCounting.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryCounting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockAdjustmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StockAdjustmentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockAdjustmentResponse>>> GetStockAdjustments()
    {
        var adjustments = await _context.StockAdjustments
            .Include(adjustment => adjustment.Product)
            .Include(adjustment => adjustment.Warehouse)
            .Include(adjustment => adjustment.StockCountSession)
            .OrderByDescending(adjustment => adjustment.ApprovedAt)
            .Select(adjustment => new StockAdjustmentResponse
            {
                Id = adjustment.Id,
                StockCountSessionId = adjustment.StockCountSessionId,
                SessionNumber = adjustment.StockCountSession.SessionNumber,
                ProductId = adjustment.ProductId,
                ProductCode = adjustment.Product.Code,
                ProductName = adjustment.Product.Name,
                WarehouseId = adjustment.WarehouseId,
                WarehouseName = adjustment.Warehouse.Name,
                OldQuantity = adjustment.OldQuantity,
                NewQuantity = adjustment.NewQuantity,
                AdjustmentQuantity = adjustment.AdjustmentQuantity,
                Reason = adjustment.Reason,
                ApprovedBy = adjustment.ApprovedBy,
                ApprovedAt = adjustment.ApprovedAt
            })
            .ToListAsync();

        return Ok(adjustments);
    }

    [HttpGet("session/{sessionId:int}")]
    public async Task<ActionResult<IEnumerable<StockAdjustmentResponse>>> GetStockAdjustmentsBySession(int sessionId)
    {
        var sessionExists = await _context.StockCountSessions
            .AnyAsync(session => session.Id == sessionId);

        if (!sessionExists)
        {
            return NotFound();
        }

        var adjustments = await _context.StockAdjustments
            .Include(adjustment => adjustment.Product)
            .Include(adjustment => adjustment.Warehouse)
            .Include(adjustment => adjustment.StockCountSession)
            .Where(adjustment => adjustment.StockCountSessionId == sessionId)
            .OrderBy(adjustment => adjustment.Product.Code)
            .Select(adjustment => new StockAdjustmentResponse
            {
                Id = adjustment.Id,
                StockCountSessionId = adjustment.StockCountSessionId,
                SessionNumber = adjustment.StockCountSession.SessionNumber,
                ProductId = adjustment.ProductId,
                ProductCode = adjustment.Product.Code,
                ProductName = adjustment.Product.Name,
                WarehouseId = adjustment.WarehouseId,
                WarehouseName = adjustment.Warehouse.Name,
                OldQuantity = adjustment.OldQuantity,
                NewQuantity = adjustment.NewQuantity,
                AdjustmentQuantity = adjustment.AdjustmentQuantity,
                Reason = adjustment.Reason,
                ApprovedBy = adjustment.ApprovedBy,
                ApprovedAt = adjustment.ApprovedAt
            })
            .ToListAsync();

        return Ok(adjustments);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StockAdjustmentResponse>> GetStockAdjustment(int id)
    {
        var adjustment = await _context.StockAdjustments
            .Include(currentAdjustment => currentAdjustment.Product)
            .Include(currentAdjustment => currentAdjustment.Warehouse)
            .Include(currentAdjustment => currentAdjustment.StockCountSession)
            .Where(currentAdjustment => currentAdjustment.Id == id)
            .Select(currentAdjustment => new StockAdjustmentResponse
            {
                Id = currentAdjustment.Id,
                StockCountSessionId = currentAdjustment.StockCountSessionId,
                SessionNumber = currentAdjustment.StockCountSession.SessionNumber,
                ProductId = currentAdjustment.ProductId,
                ProductCode = currentAdjustment.Product.Code,
                ProductName = currentAdjustment.Product.Name,
                WarehouseId = currentAdjustment.WarehouseId,
                WarehouseName = currentAdjustment.Warehouse.Name,
                OldQuantity = currentAdjustment.OldQuantity,
                NewQuantity = currentAdjustment.NewQuantity,
                AdjustmentQuantity = currentAdjustment.AdjustmentQuantity,
                Reason = currentAdjustment.Reason,
                ApprovedBy = currentAdjustment.ApprovedBy,
                ApprovedAt = currentAdjustment.ApprovedAt
            })
            .FirstOrDefaultAsync();

        if (adjustment is null)
        {
            return NotFound();
        }

        return Ok(adjustment);
    }
}
