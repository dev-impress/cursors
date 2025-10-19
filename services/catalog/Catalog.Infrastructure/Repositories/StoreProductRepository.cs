using Catalog.Domain.Abstractions;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class StoreProductRepository : IStoreProductRepository
{
    private readonly CatalogDbContext _db;
    public StoreProductRepository(CatalogDbContext db) { _db = db; }

    public async Task<StoreProduct?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _db.StoreProducts.Include(sp=>sp.Store).Include(sp=>sp.GlobalProduct).Include(sp=>sp.PriceHistory)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<StoreProduct?> GetByStoreAndNameAsync(Guid storeId, string storeName, CancellationToken ct = default)
        => await _db.StoreProducts.Include(sp=>sp.Store).Include(sp=>sp.GlobalProduct)
            .FirstOrDefaultAsync(x => x.StoreId == storeId && x.StoreSpecificName == storeName, ct);

    public async Task<List<StoreProduct>> ListByGlobalProductAsync(Guid globalProductId, CancellationToken ct = default)
        => await _db.StoreProducts.AsNoTracking().Include(sp=>sp.Store)
            .Where(sp => sp.GlobalProductId == globalProductId).ToListAsync(ct);

    public async Task<List<StoreProduct>> ListByStoreAsync(Guid storeId, CancellationToken ct = default)
        => await _db.StoreProducts.AsNoTracking().Include(sp=>sp.GlobalProduct)
            .Where(sp => sp.StoreId == storeId).ToListAsync(ct);

    public async Task AddAsync(StoreProduct entity, CancellationToken cancellationToken = default)
    {
        await _db.StoreProducts.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(StoreProduct entity, CancellationToken cancellationToken = default)
    {
        _db.StoreProducts.Update(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(StoreProduct entity, CancellationToken cancellationToken = default)
    {
        _db.StoreProducts.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
