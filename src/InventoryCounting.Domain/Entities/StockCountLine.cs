namespace InventoryCounting.Domain.Entities;

public class StockCountLine
{
    public int Id { get; set; }

    public int StockCountSessionId { get; set; }

    public StockCountSession StockCountSession { get; set; } = null!;

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public string ProductCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public decimal SystemQuantity { get; set; }

    public decimal CountedQuantity { get; set; }

    public decimal Difference { get; set; }

    public string? Notes { get; set; }
}
