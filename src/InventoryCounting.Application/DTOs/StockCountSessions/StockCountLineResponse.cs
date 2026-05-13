namespace InventoryCounting.Application.DTOs.StockCountSessions;

public class StockCountLineResponse
{
    public int Id { get; set; }

    public int StockCountSessionId { get; set; }

    public int ProductId { get; set; }

    public string ProductCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public decimal SystemQuantity { get; set; }

    public decimal CountedQuantity { get; set; }

    public decimal Difference { get; set; }

    public string? Notes { get; set; }
}
