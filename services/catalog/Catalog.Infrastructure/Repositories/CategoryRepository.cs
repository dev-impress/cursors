using Catalog.Domain.Abstractions;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly CatalogDbContext _dbContext;

    public CategoryRepository(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<List<Category>> ListAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Categories.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);

    public async Task AddAsync(Category entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Categories.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Category entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Categories.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Category entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Categories.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
