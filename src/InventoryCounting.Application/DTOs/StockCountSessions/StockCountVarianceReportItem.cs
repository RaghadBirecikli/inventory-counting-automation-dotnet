namespace InventoryCounting.Application.DTOs.StockCountSessions;

public class StockCountVarianceReportItem
{
    public string ProductCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public decimal SystemQuantity { get; set; }

    public decimal CountedQuantity { get; set; }

    public decimal Difference { get; set; }

    public string VarianceType { get; set; } = string.Empty;
}
