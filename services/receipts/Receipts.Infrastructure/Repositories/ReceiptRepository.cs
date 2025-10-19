using Microsoft.EntityFrameworkCore;
using Receipts.Domain.Abstractions;
using Receipts.Domain.Entities;
using Receipts.Infrastructure.Db;

namespace Receipts.Infrastructure.Repositories;

public class ReceiptRepository : IReceiptRepository
{
    private readonly ReceiptsDbContext _dbContext;

    public ReceiptRepository(ReceiptsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Receipt?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _dbContext.Receipts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<List<Receipt>> ListAsync(int skip = 0, int take = 100, CancellationToken ct = default)
        => await _dbContext.Receipts.AsNoTracking().OrderByDescending(x => x.IssuedAt).Skip(skip).Take(take).ToListAsync(ct);

    public async Task AddAsync(Receipt entity, CancellationToken ct = default)
    {
        await _dbContext.Receipts.AddAsync(entity, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}
