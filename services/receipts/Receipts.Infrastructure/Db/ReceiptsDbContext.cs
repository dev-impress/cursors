using Microsoft.EntityFrameworkCore;
using Receipts.Domain.Entities;

namespace Receipts.Infrastructure.Db;

public class ReceiptsDbContext : DbContext
{
    public DbSet<Receipt> Receipts => Set<Receipt>();

    public ReceiptsDbContext(DbContextOptions<ReceiptsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("receipts");
        modelBuilder.Entity<Receipt>(builder =>
        {
            builder.ToTable("receipts");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.StoreName).IsRequired().HasMaxLength(300);
            builder.Property(x => x.Currency).IsRequired().HasMaxLength(10);
            builder.Property(x => x.Total).HasColumnType("numeric(18,2)");
            builder.Property(x => x.Payload).IsRequired();
            builder.HasIndex(x => x.IssuedAt);
        });
    }
}
