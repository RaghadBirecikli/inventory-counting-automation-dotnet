using InventoryCounting.Application.DTOs.AuditLogs;
using InventoryCounting.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryCounting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditLogsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuditLogsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuditLogResponse>>> GetAuditLogs()
    {
        var auditLogs = await _context.AuditLogs
            .OrderByDescending(auditLog => auditLog.CreatedAt)
            .Select(auditLog => new AuditLogResponse
            {
                Id = auditLog.Id,
                Action = auditLog.Action,
                EntityName = auditLog.EntityName,
                EntityId = auditLog.EntityId,
                OldValue = auditLog.OldValue,
                NewValue = auditLog.NewValue,
                CreatedBy = auditLog.CreatedBy,
                CreatedAt = auditLog.CreatedAt
            })
            .ToListAsync();

        return Ok(auditLogs);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AuditLogResponse>> GetAuditLog(int id)
    {
        var auditLog = await _context.AuditLogs
            .Where(currentAuditLog => currentAuditLog.Id == id)
            .Select(currentAuditLog => new AuditLogResponse
            {
                Id = currentAuditLog.Id,
                Action = currentAuditLog.Action,
                EntityName = currentAuditLog.EntityName,
                EntityId = currentAuditLog.EntityId,
                OldValue = currentAuditLog.OldValue,
                NewValue = currentAuditLog.NewValue,
                CreatedBy = currentAuditLog.CreatedBy,
                CreatedAt = currentAuditLog.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (auditLog is null)
        {
            return NotFound();
        }

        return Ok(auditLog);
    }

    [HttpGet("entity/{entityName}/{entityId}")]
    public async Task<ActionResult<IEnumerable<AuditLogResponse>>> GetAuditLogsByEntity(
        string entityName,
        string entityId)
    {
        var auditLogs = await _context.AuditLogs
            .Where(auditLog => auditLog.EntityName == entityName && auditLog.EntityId == entityId)
            .OrderByDescending(auditLog => auditLog.CreatedAt)
            .Select(auditLog => new AuditLogResponse
            {
                Id = auditLog.Id,
                Action = auditLog.Action,
                EntityName = auditLog.EntityName,
                EntityId = auditLog.EntityId,
                OldValue = auditLog.OldValue,
                NewValue = auditLog.NewValue,
                CreatedBy = auditLog.CreatedBy,
                CreatedAt = auditLog.CreatedAt
            })
            .ToListAsync();

        return Ok(auditLogs);
    }
}
