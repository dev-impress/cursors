using Catalog.Domain.Abstractions;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class GlobalProductRepository : IGlobalProductRepository
{
    private readonly CatalogDbContext _db;
    public GlobalProductRepository(CatalogDbContext db) { _db = db; }

    public async Task<GlobalProduct?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _db.GlobalProducts.Include(g=>g.Category).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<GlobalProduct?> GetByNameAsync(string name, CancellationToken ct = default)
        => await _db.GlobalProducts.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name, ct);

    public async Task<List<GlobalProduct>> ListAsync(CancellationToken ct = default)
        => await _db.GlobalProducts.AsNoTracking().Include(g=>g.Category).OrderBy(x => x.Name).ToListAsync(ct);

    public async Task AddAsync(GlobalProduct entity, CancellationToken cancellationToken = default)
    {
        await _db.GlobalProducts.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(GlobalProduct entity, CancellationToken cancellationToken = default)
    {
        _db.GlobalProducts.Update(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(GlobalProduct entity, CancellationToken cancellationToken = default)
    {
        _db.GlobalProducts.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
