using Catalog.Domain.Entities;

namespace Catalog.Domain.Abstractions;

public interface IStoreRepository : IRepository<Store>
{
    Task<List<Store>> ListAsync(CancellationToken cancellationToken = default);
}