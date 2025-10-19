using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Db;

public class CatalogDbContext : DbContext
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<GlobalProduct> GlobalProducts => Set<GlobalProduct>();
    public DbSet<StoreProduct> StoreProducts => Set<StoreProduct>();
    public DbSet<StoreProductPriceHistory> StoreProductPriceHistories => Set<StoreProductPriceHistory>();

    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("catalog");

        modelBuilder.Entity<Category>(builder =>
        {
            builder.ToTable("categories");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(1000);
            builder.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Store>(builder =>
        {
            builder.ToTable("stores");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.HasIndex(x => x.Name).IsUnique();
            builder.HasOne(x => x.Category)
                   .WithMany()
                   .HasForeignKey(x => x.CategoryId)
                   .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Product>(builder =>
        {
            builder.ToTable("products");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Price).HasColumnType("numeric(18,2)").IsRequired();

            builder.HasOne(x => x.Store)
                   .WithMany()
                   .HasForeignKey(x => x.StoreId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Category)
                   .WithMany()
                   .HasForeignKey(x => x.CategoryId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(x => new { x.StoreId, x.Name }).IsUnique();
        });

        modelBuilder.Entity<GlobalProduct>(builder =>
        {
            builder.ToTable("global_products");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.HasIndex(x => x.Name).IsUnique();
            builder.HasOne(x => x.Category)
                   .WithMany()
                   .HasForeignKey(x => x.CategoryId)
                   .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<StoreProduct>(builder =>
        {
            builder.ToTable("store_products");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.StoreSpecificName).IsRequired().HasMaxLength(300);
            builder.Property(x => x.CurrentPrice).HasColumnType("numeric(18,2)").IsRequired();
            builder.HasOne(x => x.Store)
                   .WithMany()
                   .HasForeignKey(x => x.StoreId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.GlobalProduct)
                   .WithMany()
                   .HasForeignKey(x => x.GlobalProductId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(x => new { x.StoreId, x.StoreSpecificName }).IsUnique();
        });

        modelBuilder.Entity<StoreProductPriceHistory>(builder =>
        {
            builder.ToTable("store_product_price_history");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Price).HasColumnType("numeric(18,2)").IsRequired();
            builder.Property(x => x.ChangedAtUtc).IsRequired();
            builder.HasOne(x => x.StoreProduct)
                   .WithMany(sp => sp.PriceHistory)
                   .HasForeignKey(x => x.StoreProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
