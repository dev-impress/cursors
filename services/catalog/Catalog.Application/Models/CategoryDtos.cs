namespace Catalog.Application.Models;

public record CategoryCreateRequest(string Name, string? Description);
public record CategoryUpdateRequest(string Name, string? Description);
public record CategoryResponse(Guid Id, string Name, string? Description);
