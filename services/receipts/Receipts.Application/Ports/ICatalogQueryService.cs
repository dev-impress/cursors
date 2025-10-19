namespace Receipts.Application.Ports;

public interface ICatalogQueryService
{
    Task<IReadOnlyList<StoreDto>> GetStoresAsync(CancellationToken ct = default);
}

public record StoreDto(Guid Id, string Name, Guid? CategoryId, string? CategoryName);
