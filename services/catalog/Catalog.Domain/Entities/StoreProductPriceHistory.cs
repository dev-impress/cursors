namespace Catalog.Domain.Entities;

public class StoreProductPriceHistory
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid StoreProductId { get; private set; }
    public decimal Price { get; private set; }
    public DateTime ChangedAtUtc { get; private set; }

    public StoreProduct StoreProduct { get; private set; } = default!;

    private StoreProductPriceHistory() { }

    public StoreProductPriceHistory(decimal price, DateTime changedAtUtc)
    {
        Price = price;
        ChangedAtUtc = changedAtUtc;
    }
}
