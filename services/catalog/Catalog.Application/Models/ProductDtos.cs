namespace Catalog.Application.Models;

public record ProductCreateRequest(string Name, Guid StoreId, decimal Price, Guid? CategoryId);
public record ProductUpdateRequest(string Name, decimal Price, Guid? CategoryId);
public record ProductResponse(Guid Id, string Name, decimal Price, Guid StoreId, string StoreName, Guid? CategoryId, string? CategoryName, Guid? EffectiveCategoryId, string? EffectiveCategoryName);
