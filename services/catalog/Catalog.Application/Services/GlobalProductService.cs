using Catalog.Application.Models;
using Catalog.Domain.Abstractions;
using Catalog.Domain.Entities;

namespace Catalog.Application.Services;

public class GlobalProductService
{
    private readonly IGlobalProductRepository _globals;
    private readonly IStoreProductRepository _storeProducts;

    public GlobalProductService(IGlobalProductRepository globals, IStoreProductRepository storeProducts)
    {
        _globals = globals;
        _storeProducts = storeProducts;
    }

    public async Task<Guid> CreateAsync(GlobalProductCreateRequest request, CancellationToken ct = default)
    {
        var entity = new GlobalProduct(request.Name, request.CategoryId);
        await _globals.AddAsync(entity, ct);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, GlobalProductUpdateRequest request, CancellationToken ct = default)
    {
        var entity = await _globals.GetByIdAsync(id, ct);
        if (entity is null) return false;
        entity.Rename(request.Name);
        entity.AssignCategory(request.CategoryId);
        await _globals.UpdateAsync(entity, ct);
        return true;
    }

    public async Task<GlobalProductResponse?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var x = await _globals.GetByIdAsync(id, ct);
        return x is null ? null : new GlobalProductResponse(x.Id, x.Name, x.CategoryId, x.Category?.Name);
    }

    public async Task<GlobalProductResponse?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        var x = await _globals.GetByNameAsync(name, ct);
        return x is null ? null : new GlobalProductResponse(x.Id, x.Name, x.CategoryId, x.Category?.Name);
    }

    public async Task<List<GlobalProductResponse>> ListAsync(CancellationToken ct = default)
    {
        var items = await _globals.ListAsync(ct);
        return items.Select(x => new GlobalProductResponse(x.Id, x.Name, x.CategoryId, x.Category?.Name)).ToList();
    }

    public async Task<GlobalProductDetailsResponse?> GetDetailsAsync(Guid id, CancellationToken ct = default)
    {
        var gp = await _globals.GetByIdAsync(id, ct);
        if (gp is null) return null;
        var storeProducts = await _storeProducts.ListByGlobalProductAsync(id, ct);
        var list = storeProducts.Select(sp => new StoreProductBrief(sp.Id, sp.StoreId, sp.Store.Name, sp.StoreSpecificName, sp.CurrentPrice)).ToList();
        return new GlobalProductDetailsResponse(gp.Id, gp.Name, gp.CategoryId, gp.Category?.Name, list);
    }
}
