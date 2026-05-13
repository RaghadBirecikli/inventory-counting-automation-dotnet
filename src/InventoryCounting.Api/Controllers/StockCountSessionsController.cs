using ClosedXML.Excel;
using InventoryCounting.Application.DTOs.StockCountSessions;
using InventoryCounting.Domain.Entities;
using InventoryCounting.Domain.Enums;
using InventoryCounting.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryCounting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockCountSessionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StockCountSessionsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockCountSessionResponse>>> GetStockCountSessions()
    {
        var sessions = await _context.StockCountSessions
            .Include(session => session.Warehouse)
            .OrderByDescending(session => session.CreatedAt)
            .Select(session => new StockCountSessionResponse
            {
                Id = session.Id,
                SessionNumber = session.SessionNumber,
                WarehouseId = session.WarehouseId,
                WarehouseName = session.Warehouse.Name,
                CountDate = session.CountDate,
                Status = session.Status.ToString(),
                CreatedBy = session.CreatedBy,
                CreatedAt = session.CreatedAt,
                CompletedAt = session.CompletedAt,
                LinesCount = session.Lines.Count
            })
            .ToListAsync();

        return Ok(sessions);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StockCountSessionResponse>> GetStockCountSession(int id)
    {
        var session = await _context.StockCountSessions
            .Include(currentSession => currentSession.Warehouse)
            .Select(currentSession => new StockCountSessionResponse
            {
                Id = currentSession.Id,
                SessionNumber = currentSession.SessionNumber,
                WarehouseId = currentSession.WarehouseId,
                WarehouseName = currentSession.Warehouse.Name,
                CountDate = currentSession.CountDate,
                Status = currentSession.Status.ToString(),
                CreatedBy = currentSession.CreatedBy,
                CreatedAt = currentSession.CreatedAt,
                CompletedAt = currentSession.CompletedAt,
                LinesCount = currentSession.Lines.Count
            })
            .FirstOrDefaultAsync(currentSession => currentSession.Id == id);

        if (session is null)
        {
            return NotFound();
        }

        return Ok(session);
    }

    [HttpPost]
    public async Task<ActionResult<StockCountSessionResponse>> CreateStockCountSession(CreateStockCountSessionRequest request)
    {
        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(currentWarehouse => currentWarehouse.Id == request.WarehouseId && currentWarehouse.IsActive);

        if (warehouse is null)
        {
            ModelState.AddModelError(nameof(request.WarehouseId), "Warehouse must exist and be active.");
        }

        if (request.CountDate == default)
        {
            ModelState.AddModelError(nameof(request.CountDate), "CountDate is required.");
        }

        if (string.IsNullOrWhiteSpace(request.CreatedBy))
        {
            ModelState.AddModelError(nameof(request.CreatedBy), "CreatedBy is required.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var currentYear = DateTime.UtcNow.Year;
        var yearPrefix = $"SC-{currentYear}-";
        var sessionCount = await _context.StockCountSessions
            .CountAsync(session => session.SessionNumber.StartsWith(yearPrefix));

        var session = new StockCountSession
        {
            SessionNumber = $"{yearPrefix}{sessionCount + 1:0000}",
            WarehouseId = request.WarehouseId,
            CountDate = request.CountDate,
            Status = StockCountStatus.Draft,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            CompletedAt = null
        };

        _context.StockCountSessions.Add(session);
        await _context.SaveChangesAsync();

        var response = new StockCountSessionResponse
        {
            Id = session.Id,
            SessionNumber = session.SessionNumber,
            WarehouseId = session.WarehouseId,
            WarehouseName = warehouse!.Name,
            CountDate = session.CountDate,
            Status = session.Status.ToString(),
            CreatedBy = session.CreatedBy,
            CreatedAt = session.CreatedAt,
            CompletedAt = session.CompletedAt,
            LinesCount = 0
        };

        return CreatedAtAction(nameof(GetStockCountSession), new { id = response.Id }, response);
    }

    [HttpGet("{id:int}/lines")]
    public async Task<ActionResult<IEnumerable<StockCountLineResponse>>> GetStockCountSessionLines(int id)
    {
        var sessionExists = await _context.StockCountSessions
            .AnyAsync(session => session.Id == id);

        if (!sessionExists)
        {
            return NotFound();
        }

        var lines = await _context.StockCountLines
            .Where(line => line.StockCountSessionId == id)
            .OrderBy(line => line.ProductCode)
            .Select(line => new StockCountLineResponse
            {
                Id = line.Id,
                StockCountSessionId = line.StockCountSessionId,
                ProductId = line.ProductId,
                ProductCode = line.ProductCode,
                ProductName = line.ProductName,
                SystemQuantity = line.SystemQuantity,
                CountedQuantity = line.CountedQuantity,
                Difference = line.Difference,
                Notes = line.Notes
            })
            .ToListAsync();

        return Ok(lines);
    }

    [HttpGet("{id:int}/variance-report")]
    public async Task<ActionResult<IEnumerable<StockCountVarianceReportItem>>> GetVarianceReport(int id)
    {
        var sessionExists = await _context.StockCountSessions
            .AnyAsync(session => session.Id == id);

        if (!sessionExists)
        {
            return NotFound();
        }

        var lines = await _context.StockCountLines
            .Where(line => line.StockCountSessionId == id)
            .OrderBy(line => line.ProductCode)
            .ToListAsync();

        if (lines.Count == 0)
        {
            return BadRequest("No stock count lines found. Please upload an Excel file first.");
        }

        var report = lines.Select(line => new StockCountVarianceReportItem
        {
            ProductCode = line.ProductCode,
            ProductName = line.ProductName,
            SystemQuantity = line.SystemQuantity,
            CountedQuantity = line.CountedQuantity,
            Difference = line.Difference,
            VarianceType = line.Difference < 0
                ? "Shortage"
                : line.Difference > 0
                    ? "Surplus"
                    : "Matched"
        });

        return Ok(report);
    }

    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> ApproveStockCountSession(int id, ApproveStockCountSessionRequest request)
    {
        var session = await _context.StockCountSessions
            .Include(currentSession => currentSession.Lines)
            .FirstOrDefaultAsync(currentSession => currentSession.Id == id);

        if (session is null)
        {
            return NotFound();
        }

        if (session.Status == StockCountStatus.Approved)
        {
            return BadRequest("Stock count session is already approved.");
        }

        if (session.Status == StockCountStatus.Rejected)
        {
            return BadRequest("Rejected stock count sessions cannot be approved.");
        }

        if (session.Status != StockCountStatus.Uploaded && session.Status != StockCountStatus.Reviewed)
        {
            return BadRequest("Stock count session must be Uploaded or Reviewed before approval.");
        }

        if (session.Lines.Count == 0)
        {
            return BadRequest("Stock count session must have lines before approval.");
        }

        if (string.IsNullOrWhiteSpace(request.ApprovedBy))
        {
            ModelState.AddModelError(nameof(request.ApprovedBy), "ApprovedBy is required.");
            return ValidationProblem(ModelState);
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        var approvedAt = DateTime.UtcNow;
        var adjustmentsCreated = 0;

        foreach (var line in session.Lines.Where(currentLine => currentLine.Difference != 0))
        {
            var stockBalance = await _context.StockBalances
                .FirstOrDefaultAsync(currentStockBalance =>
                    currentStockBalance.ProductId == line.ProductId
                    && currentStockBalance.WarehouseId == session.WarehouseId);

            if (stockBalance is null)
            {
                return BadRequest($"Stock balance is missing for product '{line.ProductCode}' in this session warehouse.");
            }

            var oldQuantity = stockBalance.Quantity;
            var newQuantity = line.CountedQuantity;
            var adjustmentQuantity = newQuantity - oldQuantity;

            stockBalance.Quantity = newQuantity;
            stockBalance.LastUpdatedAt = approvedAt;

            _context.StockAdjustments.Add(new StockAdjustment
            {
                StockCountSessionId = session.Id,
                ProductId = line.ProductId,
                WarehouseId = session.WarehouseId,
                OldQuantity = oldQuantity,
                NewQuantity = newQuantity,
                AdjustmentQuantity = adjustmentQuantity,
                Reason = "Inventory count approval",
                ApprovedBy = request.ApprovedBy,
                ApprovedAt = approvedAt
            });

            adjustmentsCreated++;
        }

        session.Status = StockCountStatus.Approved;
        session.CompletedAt = approvedAt;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return Ok(new
        {
            sessionId = session.Id,
            sessionNumber = session.SessionNumber,
            status = session.Status.ToString(),
            approvedBy = request.ApprovedBy,
            approvedAt,
            adjustmentsCreated,
            message = "Stock count session approved successfully."
        });
    }

    [HttpPost("{id:int}/upload-excel")]
    public async Task<IActionResult> UploadExcel(int id, IFormFile file)
    {
        var session = await _context.StockCountSessions
            .FirstOrDefaultAsync(currentSession => currentSession.Id == id);

        if (session is null)
        {
            return NotFound();
        }

        if (session.Status != StockCountStatus.Draft)
        {
            return BadRequest("Stock count session must be in Draft status before uploading Excel.");
        }

        if (file is null)
        {
            return BadRequest("Excel file is required.");
        }

        if (file.Length == 0)
        {
            return BadRequest("Excel file must not be empty.");
        }

        var extension = Path.GetExtension(file.FileName);
        if (!string.Equals(extension, ".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Only .xlsx files are allowed.");
        }

        using var stream = file.OpenReadStream();
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();

        var productCodeHeader = worksheet.Cell(1, 1).GetString().Trim();
        var countedQuantityHeader = worksheet.Cell(1, 2).GetString().Trim();

        if (!string.Equals(productCodeHeader, "ProductCode", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(countedQuantityHeader, "CountedQuantity", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Excel headers must be ProductCode and CountedQuantity in row 1.");
        }

        var newLines = new List<StockCountLine>();
        var rowNumber = 2;

        while (true)
        {
            var productCode = worksheet.Cell(rowNumber, 1).GetString().Trim();

            if (string.IsNullOrWhiteSpace(productCode))
            {
                break;
            }

            var countedQuantityCell = worksheet.Cell(rowNumber, 2);

            if (!countedQuantityCell.TryGetValue<decimal>(out var countedQuantity))
            {
                return BadRequest($"Row {rowNumber}: CountedQuantity must be numeric.");
            }

            if (countedQuantity < 0)
            {
                return BadRequest($"Row {rowNumber}: CountedQuantity cannot be negative.");
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(currentProduct => currentProduct.Code == productCode && currentProduct.IsActive);

            if (product is null)
            {
                return BadRequest($"Row {rowNumber}: ProductCode '{productCode}' does not exist or is inactive.");
            }

            var stockBalance = await _context.StockBalances
                .FirstOrDefaultAsync(currentStockBalance =>
                    currentStockBalance.ProductId == product.Id
                    && currentStockBalance.WarehouseId == session.WarehouseId);

            if (stockBalance is null)
            {
                return BadRequest($"Row {rowNumber}: Stock balance does not exist for product '{productCode}' in this session warehouse.");
            }

            var systemQuantity = stockBalance.Quantity;

            newLines.Add(new StockCountLine
            {
                StockCountSessionId = session.Id,
                ProductId = product.Id,
                ProductCode = product.Code,
                ProductName = product.Name,
                SystemQuantity = systemQuantity,
                CountedQuantity = countedQuantity,
                Difference = countedQuantity - systemQuantity
            });

            rowNumber++;
        }

        var existingLines = await _context.StockCountLines
            .Where(line => line.StockCountSessionId == session.Id)
            .ToListAsync();

        _context.StockCountLines.RemoveRange(existingLines);
        _context.StockCountLines.AddRange(newLines);

        session.Status = StockCountStatus.Uploaded;

        await _context.SaveChangesAsync();

        var uploadedAt = DateTime.UtcNow;

        return Ok(new
        {
            sessionId = session.Id,
            sessionNumber = session.SessionNumber,
            totalLines = newLines.Count,
            uploadedAt,
            message = "Excel file uploaded successfully."
        });
    }
}
