namespace Catalog.Application.Models;

public record StoreProductCreateRequest(Guid GlobalProductId, string StoreSpecificName, decimal Price);
public record StoreProductRenameRequest(string StoreSpecificName);
public record StoreProductPriceUpdateRequest(decimal Price);
public record StoreProductResponse(Guid Id, Guid StoreId, string StoreName, Guid GlobalProductId, string GlobalProductName, string StoreSpecificName, decimal CurrentPrice);
public record StoreProductWithHistoryResponse(Guid Id, Guid StoreId, string StoreName, Guid GlobalProductId, string GlobalProductName, string StoreSpecificName, decimal CurrentPrice, List<PricePoint> History);
public record PricePoint(DateTime ChangedAtUtc, decimal Price);
