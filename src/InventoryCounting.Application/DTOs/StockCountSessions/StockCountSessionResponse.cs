namespace InventoryCounting.Application.DTOs.StockCountSessions;

public class StockCountSessionResponse
{
    public int Id { get; set; }

    public string SessionNumber { get; set; } = string.Empty;

    public int WarehouseId { get; set; }

    public string WarehouseName { get; set; } = string.Empty;

    public DateTime CountDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int LinesCount { get; set; }
}
