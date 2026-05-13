namespace InventoryCounting.Domain.Entities;

public class StockAdjustment
{
    public int Id { get; set; }

    public int StockCountSessionId { get; set; }

    public StockCountSession StockCountSession { get; set; } = null!;

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public int WarehouseId { get; set; }

    public Warehouse Warehouse { get; set; } = null!;

    public decimal OldQuantity { get; set; }

    public decimal NewQuantity { get; set; }

    public decimal AdjustmentQuantity { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string ApprovedBy { get; set; } = string.Empty;

    public DateTime ApprovedAt { get; set; }
}
