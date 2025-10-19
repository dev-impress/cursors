using Catalog.Application.Models;
using Catalog.Domain.Abstractions;
using Catalog.Domain.Entities;

namespace Catalog.Application.Services;

public class StoreProductService
{
    private readonly IStoreRepository _stores;
    private readonly IGlobalProductRepository _globals;
    private readonly IStoreProductRepository _storeProducts;

    public StoreProductService(IStoreRepository stores, IGlobalProductRepository globals, IStoreProductRepository storeProducts)
    {
        _stores = stores;
        _globals = globals;
        _storeProducts = storeProducts;
    }

    public async Task<StoreProductResponse?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var x = await _storeProducts.GetByIdAsync(id, ct);
        return x is null ? null : new StoreProductResponse(x.Id, x.StoreId, x.Store.Name, x.GlobalProductId, x.GlobalProduct.Name, x.StoreSpecificName, x.CurrentPrice);
    }

    public async Task<StoreProductWithHistoryResponse?> GetWithHistoryAsync(Guid id, CancellationToken ct = default)
    {
        var x = await _storeProducts.GetByIdAsync(id, ct);
        if (x is null) return null;
        return new StoreProductWithHistoryResponse(
            x.Id, x.StoreId, x.Store.Name, x.GlobalProductId, x.GlobalProduct.Name,
            x.StoreSpecificName, x.CurrentPrice,
            x.PriceHistory.OrderBy(ph => ph.ChangedAtUtc).Select(ph => new PricePoint(ph.ChangedAtUtc, ph.Price)).ToList()
        );
    }

    public async Task<Guid> CreateAsync(Guid storeId, StoreProductCreateRequest req, CancellationToken ct = default)
    {
        var store = await _stores.GetByIdAsync(storeId, ct) ?? throw new ArgumentException("Store not found", nameof(storeId));
        var gp = await _globals.GetByIdAsync(req.GlobalProductId, ct) ?? throw new ArgumentException("GlobalProduct not found", nameof(req.GlobalProductId));
        var existing = await _storeProducts.GetByStoreAndNameAsync(storeId, req.StoreSpecificName, ct);
        if (existing is not null) return existing.Id;
        var sp = new StoreProduct(storeId, req.GlobalProductId, req.StoreSpecificName, req.Price);
        await _storeProducts.AddAsync(sp, ct);
        return sp.Id;
    }

    public async Task<bool> RenameAsync(Guid id, StoreProductRenameRequest req, CancellationToken ct = default)
    {
        var sp = await _storeProducts.GetByIdAsync(id, ct);
        if (sp is null) return false;
        sp.RenameInStore(req.StoreSpecificName);
        await _storeProducts.UpdateAsync(sp, ct);
        return true;
    }

    public async Task<bool> UpdatePriceAsync(Guid id, StoreProductPriceUpdateRequest req, CancellationToken ct = default)
    {
        var sp = await _storeProducts.GetByIdAsync(id, ct);
        if (sp is null) return false;
        sp.ChangePrice(req.Price);
        await _storeProducts.UpdateAsync(sp, ct);
        return true;
    }
}
