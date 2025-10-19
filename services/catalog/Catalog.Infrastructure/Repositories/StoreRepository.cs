using Catalog.Domain.Abstractions;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class StoreRepository : IStoreRepository
{
    private readonly CatalogDbContext _dbContext;

    public StoreRepository(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Store?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbContext.Stores.Include(s => s.Category).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<List<Store>> ListAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Stores.AsNoTracking().Include(s => s.Category).OrderBy(x => x.Name).ToListAsync(cancellationToken);

    public async Task AddAsync(Store entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Stores.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Store entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Stores.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Store entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Stores.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
