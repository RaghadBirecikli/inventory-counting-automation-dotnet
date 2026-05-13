namespace InventoryCounting.Domain.Entities;

public class Warehouse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<StockBalance> StockBalances { get; set; } = new List<StockBalance>();
}
