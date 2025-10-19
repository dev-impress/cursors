using Catalog.Domain.Abstractions;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly CatalogDbContext _dbContext;

    public ProductRepository(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbContext.Products.Include(p => p.Category).Include(p => p.Store).ThenInclude(s => s.Category)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<List<Product>> ListAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Products.AsNoTracking().Include(p => p.Category).Include(p => p.Store).ThenInclude(s => s.Category)
            .OrderBy(x => x.Name).ToListAsync(cancellationToken);

    public async Task<List<Product>> ListByStoreAsync(Guid storeId, CancellationToken cancellationToken = default)
        => await _dbContext.Products.AsNoTracking().Include(p => p.Category).Include(p => p.Store).ThenInclude(s => s.Category)
            .Where(x => x.StoreId == storeId).OrderBy(x => x.Name).ToListAsync(cancellationToken);

    public async Task AddAsync(Product entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Products.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Products.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Product entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Products.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
