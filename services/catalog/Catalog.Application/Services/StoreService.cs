using Catalog.Application.Models;
using Catalog.Domain.Abstractions;
using Catalog.Domain.Entities;

namespace Catalog.Application.Services;

public class StoreService
{
    private readonly IStoreRepository _stores;
    private readonly ICategoryRepository _categories;

    public StoreService(IStoreRepository stores, ICategoryRepository categories)
    {
        _stores = stores;
        _categories = categories;
    }

    public async Task<List<StoreResponse>> ListAsync(CancellationToken ct = default)
    {
        var items = await _stores.ListAsync(ct);
        return items.Select(x => new StoreResponse(x.Id, x.Name, x.CategoryId, x.Category?.Name)).ToList();
    }

    public async Task<StoreResponse?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var x = await _stores.GetByIdAsync(id, ct);
        return x is null ? null : new StoreResponse(x.Id, x.Name, x.CategoryId, x.Category?.Name);
    }

    public async Task<Guid> CreateAsync(StoreCreateRequest request, CancellationToken ct = default)
    {
        if (request.CategoryId.HasValue)
        {
            var cat = await _categories.GetByIdAsync(request.CategoryId.Value, ct);
            if (cat is null) throw new ArgumentException("Category not found", nameof(request.CategoryId));
        }
        var entity = new Store(request.Name, request.CategoryId);
        await _stores.AddAsync(entity, ct);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, StoreUpdateRequest request, CancellationToken ct = default)
    {
        var entity = await _stores.GetByIdAsync(id, ct);
        if (entity is null) return false;
        if (request.CategoryId.HasValue)
        {
            var cat = await _categories.GetByIdAsync(request.CategoryId.Value, ct);
            if (cat is null) throw new ArgumentException("Category not found", nameof(request.CategoryId));
        }
        entity.Rename(request.Name);
        entity.AssignCategory(request.CategoryId);
        await _stores.UpdateAsync(entity, ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _stores.GetByIdAsync(id, ct);
        if (entity is null) return false;
        await _stores.DeleteAsync(entity, ct);
        return true;
    }
}
