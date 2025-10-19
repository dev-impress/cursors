using Catalog.Domain.Entities;

namespace Catalog.Domain.Abstractions;

public interface IProductRepository : IRepository<Product>
{
    Task<List<Product>> ListAsync(CancellationToken cancellationToken = default);
    Task<List<Product>> ListByStoreAsync(Guid storeId, CancellationToken cancellationToken = default);
}
