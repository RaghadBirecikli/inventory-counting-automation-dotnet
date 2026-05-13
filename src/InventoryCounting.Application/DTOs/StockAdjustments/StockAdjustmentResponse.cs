namespace InventoryCounting.Application.DTOs.StockAdjustments;

public class StockAdjustmentResponse
{
    public int Id { get; set; }

    public int StockCountSessionId { get; set; }

    public string SessionNumber { get; set; } = string.Empty;

    public int ProductId { get; set; }

    public string ProductCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public int WarehouseId { get; set; }

    public string WarehouseName { get; set; } = string.Empty;

    public decimal OldQuantity { get; set; }

    public decimal NewQuantity { get; set; }

    public decimal AdjustmentQuantity { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string ApprovedBy { get; set; } = string.Empty;

    public DateTime ApprovedAt { get; set; }
}
