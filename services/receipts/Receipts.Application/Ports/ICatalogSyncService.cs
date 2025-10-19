namespace Receipts.Application.Ports;

public interface ICatalogSyncService
{
    Task EnsureStoreAndProductsAsync(string storeName, IEnumerable<(string name, decimal price)> items, CancellationToken ct = default);
}
