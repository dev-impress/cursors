using Catalog.Domain.Entities;

namespace Catalog.Domain.Abstractions;

public interface ICategoryRepository : IRepository<Category>
{
    Task<List<Category>> ListAsync(CancellationToken cancellationToken = default);
}