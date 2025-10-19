using Receipts.Application.Models;
using Receipts.Domain.Abstractions;
using Receipts.Domain.Entities;

namespace Receipts.Application.Services;

public class ReceiptService
{
    private readonly IReceiptRepository _repo;

    public ReceiptService(IReceiptRepository repo)
    {
        _repo = repo;
    }

    public async Task<Guid> CreateAsync(ReceiptCreateRequest request, CancellationToken ct = default)
    {
        var entity = new Receipt(request.IssuedAt, request.StoreName, request.Currency, request.Total, request.Payload);
        await _repo.AddAsync(entity, ct);
        return entity.Id;
    }

    public async Task<ReceiptResponse?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var x = await _repo.GetByIdAsync(id, ct);
        return x is null ? null : new ReceiptResponse(x.Id, x.IssuedAt, x.StoreName, x.Currency, x.Total, x.Payload);
    }

    public async Task<List<ReceiptResponse>> ListAsync(int skip = 0, int take = 100, CancellationToken ct = default)
    {
        var items = await _repo.ListAsync(skip, take, ct);
        return items.Select(x => new ReceiptResponse(x.Id, x.IssuedAt, x.StoreName, x.Currency, x.Total, x.Payload)).ToList();
    }
}
