using Catalog.Domain.Entities;

namespace Catalog.Domain.Abstractions;

public interface IGlobalProductRepository : IRepository<GlobalProduct>
{
    Task<GlobalProduct?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<List<GlobalProduct>> ListAsync(CancellationToken ct = default);
}
