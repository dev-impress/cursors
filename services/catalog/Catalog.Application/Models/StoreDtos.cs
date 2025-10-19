namespace Catalog.Application.Models;

public record StoreCreateRequest(string Name, Guid? CategoryId);
public record StoreUpdateRequest(string Name, Guid? CategoryId);
public record StoreResponse(Guid Id, string Name, Guid? CategoryId, string? CategoryName);
