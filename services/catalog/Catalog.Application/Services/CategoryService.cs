using Catalog.Application.Models;
using Catalog.Domain.Abstractions;
using Catalog.Domain.Entities;

namespace Catalog.Application.Services;

public class CategoryService
{
    private readonly ICategoryRepository _repo;

    public CategoryService(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<CategoryResponse>> ListAsync(CancellationToken ct = default)
    {
        var items = await _repo.ListAsync(ct);
        return items.Select(x => new CategoryResponse(x.Id, x.Name, x.Description)).ToList();
    }

    public async Task<CategoryResponse?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var x = await _repo.GetByIdAsync(id, ct);
        return x is null ? null : new CategoryResponse(x.Id, x.Name, x.Description);
    }

    public async Task<Guid> CreateAsync(CategoryCreateRequest request, CancellationToken ct = default)
    {
        var entity = new Category(request.Name, request.Description);
        await _repo.AddAsync(entity, ct);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, CategoryUpdateRequest request, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        if (entity is null) return false;
        entity.Rename(request.Name);
        entity.UpdateDescription(request.Description);
        await _repo.UpdateAsync(entity, ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        if (entity is null) return false;
        await _repo.DeleteAsync(entity, ct);
        return true;
    }
}
