using InventoryCounting.Domain.Enums;

namespace InventoryCounting.Domain.Entities;

public class StockCountSession
{
    public int Id { get; set; }

    public string SessionNumber { get; set; } = string.Empty;

    public int WarehouseId { get; set; }

    public Warehouse Warehouse { get; set; } = null!;

    public DateTime CountDate { get; set; }

    public StockCountStatus Status { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    public ICollection<StockCountLine> Lines { get; set; } = new List<StockCountLine>();
}
