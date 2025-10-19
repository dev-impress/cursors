namespace Catalog.Domain.Entities;

public class StoreProduct
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid StoreId { get; private set; }
    public Guid GlobalProductId { get; private set; }
    public string StoreSpecificName { get; private set; }
    public decimal CurrentPrice { get; private set; }

    public Store Store { get; private set; } = default!;
    public GlobalProduct GlobalProduct { get; private set; } = default!;

    private readonly List<StoreProductPriceHistory> _priceHistory = new();
    public IReadOnlyCollection<StoreProductPriceHistory> PriceHistory => _priceHistory;

    private StoreProduct() { StoreSpecificName = string.Empty; }

    public StoreProduct(Guid storeId, Guid globalProductId, string storeSpecificName, decimal currentPrice)
    {
        if (string.IsNullOrWhiteSpace(storeSpecificName)) throw new ArgumentException("Name required", nameof(storeSpecificName));
        if (currentPrice < 0) throw new ArgumentOutOfRangeException(nameof(currentPrice));
        StoreId = storeId;
        GlobalProductId = globalProductId;
        StoreSpecificName = storeSpecificName.Trim();
        CurrentPrice = currentPrice;
        _priceHistory.Add(new StoreProductPriceHistory(CurrentPrice, DateTime.UtcNow));
    }

    public void RenameInStore(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required", nameof(name));
        StoreSpecificName = name.Trim();
    }

    public void ChangePrice(decimal newPrice)
    {
        if (newPrice < 0) throw new ArgumentOutOfRangeException(nameof(newPrice));
        if (newPrice == CurrentPrice) return;
        CurrentPrice = newPrice;
        _priceHistory.Add(new StoreProductPriceHistory(CurrentPrice, DateTime.UtcNow));
    }
}
