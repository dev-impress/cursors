using System.Net.Http.Json;
using Receipts.Application.Ports;

namespace Receipts.Infrastructure.CatalogSync;

public class CatalogQueryService : ICatalogQueryService
{
    private readonly HttpClient _http;
    private readonly string _catalogBaseUrl;

    public CatalogQueryService(HttpClient httpClient, IConfiguration configuration)
    {
        _http = httpClient;
        _catalogBaseUrl = configuration["CATALOG_API"] ?? configuration.GetValue<string>("Catalog:BaseUrl") ?? "http://catalog-api:8080";
    }

    public async Task<IReadOnlyList<StoreDto>> GetStoresAsync(CancellationToken ct = default)
    {
        var stores = await _http.GetFromJsonAsync<List<StoreDto>>($"{_catalogBaseUrl}/api/stores", ct);
        return stores ?? new List<StoreDto>();
    }
}
