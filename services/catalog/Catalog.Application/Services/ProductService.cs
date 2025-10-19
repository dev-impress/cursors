using Catalog.Application.Models;
using Catalog.Domain.Abstractions;
using Catalog.Domain.Entities;

namespace Catalog.Application.Services;

public class ProductService
{
    private readonly IProductRepository _products;
    private readonly IStoreRepository _stores;
    private readonly ICategoryRepository _categories;

    public ProductService(IProductRepository products, IStoreRepository stores, ICategoryRepository categories)
    {
        _products = products;
        _stores = stores;
        _categories = categories;
    }

    public async Task<List<ProductResponse>> ListAsync(CancellationToken ct = default)
    {
        var items = await _products.ListAsync(ct);
        return items.Select(ToResponse).ToList();
    }

    public async Task<ProductResponse?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var x = await _products.GetByIdAsync(id, ct);
        return x is null ? null : ToResponse(x);
    }

    public async Task<List<ProductResponse>> ListByStoreAsync(Guid storeId, CancellationToken ct = default)
    {
        var items = await _products.ListByStoreAsync(storeId, ct);
        return items.Select(ToResponse).ToList();
    }

    public async Task<Guid> CreateAsync(ProductCreateRequest request, CancellationToken ct = default)
    {
        var store = await _stores.GetByIdAsync(request.StoreId, ct) ?? throw new ArgumentException("Store not found", nameof(request.StoreId));
        if (request.CategoryId.HasValue)
        {
            var cat = await _categories.GetByIdAsync(request.CategoryId.Value, ct);
            if (cat is null) throw new ArgumentException("Category not found", nameof(request.CategoryId));
        }
        var entity = new Product(request.Name, request.StoreId, request.Price, request.CategoryId);
        await _products.AddAsync(entity, ct);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, ProductUpdateRequest request, CancellationToken ct = default)
    {
        var entity = await _products.GetByIdAsync(id, ct);
        if (entity is null) return false;
        if (request.CategoryId.HasValue)
        {
            var cat = await _categories.GetByIdAsync(request.CategoryId.Value, ct);
            if (cat is null) throw new ArgumentException("Category not found", nameof(request.CategoryId));
        }
        entity.Rename(request.Name);
        entity.ChangePrice(request.Price);
        entity.AssignCategory(request.CategoryId);
        await _products.UpdateAsync(entity, ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _products.GetByIdAsync(id, ct);
        if (entity is null) return false;
        await _products.DeleteAsync(entity, ct);
        return true;
    }

    private static ProductResponse ToResponse(Product p)
    {
        var effectiveCategoryId = p.CategoryId ?? p.Store.CategoryId;
        var effectiveCategoryName = p.CategoryId.HasValue ? p.Category?.Name : p.Store.Category?.Name;
        return new ProductResponse(p.Id, p.Name, p.Price, p.StoreId, p.Store.Name,
            p.CategoryId, p.Category?.Name, effectiveCategoryId, effectiveCategoryName);
    }
}
