namespace Catalog.Application.Models;

public record GlobalProductCreateRequest(string Name, Guid? CategoryId);
public record GlobalProductUpdateRequest(string Name, Guid? CategoryId);
public record GlobalProductResponse(Guid Id, string Name, Guid? CategoryId, string? CategoryName);

public record GlobalProductDetailsResponse(
    Guid Id,
    string Name,
    Guid? CategoryId,
    string? CategoryName,
    List<StoreProductBrief> StoreProducts
);

public record StoreProductBrief(Guid Id, Guid StoreId, string StoreName, string StoreSpecificName, decimal CurrentPrice);
