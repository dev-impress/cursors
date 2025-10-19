using Catalog.Domain.Entities;

namespace Catalog.Domain.Abstractions;

public interface IStoreProductRepository : IRepository<StoreProduct>
{
    Task<StoreProduct?> GetByStoreAndNameAsync(Guid storeId, string storeName, CancellationToken ct = default);
    Task<List<StoreProduct>> ListByGlobalProductAsync(Guid globalProductId, CancellationToken ct = default);
    Task<List<StoreProduct>> ListByStoreAsync(Guid storeId, CancellationToken ct = default);
}
