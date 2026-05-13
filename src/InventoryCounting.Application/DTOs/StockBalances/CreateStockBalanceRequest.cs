namespace InventoryCounting.Application.DTOs.StockBalances;

public class CreateStockBalanceRequest
{
    public int ProductId { get; set; }

    public int WarehouseId { get; set; }

    public decimal Quantity { get; set; }
}
