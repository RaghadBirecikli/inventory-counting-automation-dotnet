using InventoryCounting.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryCounting.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    public DbSet<Warehouse> Warehouses { get; set; }

    public DbSet<StockBalance> StockBalances { get; set; }

    public DbSet<StockCountSession> StockCountSessions { get; set; }

    public DbSet<StockCountLine> StockCountLines { get; set; }

    public DbSet<StockAdjustment> StockAdjustments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(product => product.Id);

            entity.Property(product => product.Code)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(product => product.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(product => product.Unit)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(product => product.Code)
                .IsUnique();
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(warehouse => warehouse.Id);

            entity.Property(warehouse => warehouse.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(warehouse => warehouse.Location)
                .HasMaxLength(300);
        });

        modelBuilder.Entity<StockBalance>(entity =>
        {
            entity.HasKey(stockBalance => stockBalance.Id);

            entity.Property(stockBalance => stockBalance.Quantity)
                .HasColumnType("decimal(18,2)");

            entity.HasOne(stockBalance => stockBalance.Product)
                .WithMany(product => product.StockBalances)
                .HasForeignKey(stockBalance => stockBalance.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(stockBalance => stockBalance.Warehouse)
                .WithMany(warehouse => warehouse.StockBalances)
                .HasForeignKey(stockBalance => stockBalance.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(stockBalance => new { stockBalance.ProductId, stockBalance.WarehouseId })
                .IsUnique();
        });

        modelBuilder.Entity<StockCountSession>(entity =>
        {
            entity.HasKey(stockCountSession => stockCountSession.Id);

            entity.Property(stockCountSession => stockCountSession.SessionNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(stockCountSession => stockCountSession.CreatedBy)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasOne(stockCountSession => stockCountSession.Warehouse)
                .WithMany()
                .HasForeignKey(stockCountSession => stockCountSession.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(stockCountSession => stockCountSession.SessionNumber)
                .IsUnique();
        });

        modelBuilder.Entity<StockCountLine>(entity =>
        {
            entity.HasKey(stockCountLine => stockCountLine.Id);

            entity.Property(stockCountLine => stockCountLine.ProductCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(stockCountLine => stockCountLine.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(stockCountLine => stockCountLine.SystemQuantity)
                .HasColumnType("decimal(18,2)");

            entity.Property(stockCountLine => stockCountLine.CountedQuantity)
                .HasColumnType("decimal(18,2)");

            entity.Property(stockCountLine => stockCountLine.Difference)
                .HasColumnType("decimal(18,2)");

            entity.Property(stockCountLine => stockCountLine.Notes)
                .HasMaxLength(500);

            entity.HasOne(stockCountLine => stockCountLine.StockCountSession)
                .WithMany(stockCountSession => stockCountSession.Lines)
                .HasForeignKey(stockCountLine => stockCountLine.StockCountSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(stockCountLine => stockCountLine.Product)
                .WithMany()
                .HasForeignKey(stockCountLine => stockCountLine.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StockAdjustment>(entity =>
        {
            entity.HasKey(stockAdjustment => stockAdjustment.Id);

            entity.Property(stockAdjustment => stockAdjustment.OldQuantity)
                .HasColumnType("decimal(18,2)");

            entity.Property(stockAdjustment => stockAdjustment.NewQuantity)
                .HasColumnType("decimal(18,2)");

            entity.Property(stockAdjustment => stockAdjustment.AdjustmentQuantity)
                .HasColumnType("decimal(18,2)");

            entity.Property(stockAdjustment => stockAdjustment.Reason)
                .IsRequired()
                .HasMaxLength(300);

            entity.Property(stockAdjustment => stockAdjustment.ApprovedBy)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasOne(stockAdjustment => stockAdjustment.StockCountSession)
                .WithMany()
                .HasForeignKey(stockAdjustment => stockAdjustment.StockCountSessionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(stockAdjustment => stockAdjustment.Product)
                .WithMany()
                .HasForeignKey(stockAdjustment => stockAdjustment.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(stockAdjustment => stockAdjustment.Warehouse)
                .WithMany()
                .HasForeignKey(stockAdjustment => stockAdjustment.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
