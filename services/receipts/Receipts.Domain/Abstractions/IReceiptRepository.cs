using Receipts.Domain.Entities;

namespace Receipts.Domain.Abstractions;

public interface IReceiptRepository
{
    Task<Receipt?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Receipt>> ListAsync(int skip = 0, int take = 100, CancellationToken ct = default);
    Task AddAsync(Receipt entity, CancellationToken ct = default);
}
