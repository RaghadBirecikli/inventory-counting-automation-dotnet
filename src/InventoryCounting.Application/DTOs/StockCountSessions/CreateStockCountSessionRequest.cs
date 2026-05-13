namespace InventoryCounting.Application.DTOs.StockCountSessions;

public class CreateStockCountSessionRequest
{
    public int WarehouseId { get; set; }

    public DateTime CountDate { get; set; }

    public string CreatedBy { get; set; } = string.Empty;
}
