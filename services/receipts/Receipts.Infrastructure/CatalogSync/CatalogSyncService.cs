using System.Net.Http.Json;
using Receipts.Application.Ports;

namespace Receipts.Infrastructure.CatalogSync;

public class CatalogSyncService : ICatalogSyncService
{
    private readonly HttpClient _http;
    private readonly string _catalogBaseUrl;

    public CatalogSyncService(HttpClient httpClient, IConfiguration configuration)
    {
        _http = httpClient;
        _catalogBaseUrl = configuration["CATALOG_API"] ?? configuration.GetValue<string>("Catalog:BaseUrl") ?? "http://catalog-api:8080";
    }

    public async Task EnsureStoreAndProductsAsync(string storeName, IEnumerable<(string name, decimal price)> items, CancellationToken ct = default)
    {
        // 1) find or create store
        var stores = await _http.GetFromJsonAsync<List<StoreDto>>($"{_catalogBaseUrl}/api/stores", ct) ?? new List<StoreDto>();
        var store = stores.FirstOrDefault(s => string.Equals(s.Name, storeName, StringComparison.OrdinalIgnoreCase));
        if (store is null)
        {
            var createRes = await _http.PostAsJsonAsync($"{_catalogBaseUrl}/api/stores", new { name = storeName, categoryId = (Guid?)null }, ct);
            createRes.EnsureSuccessStatusCode();
            var created = await createRes.Content.ReadFromJsonAsync<CreatedResponse>(cancellationToken: ct);
            if (created is null) return; // nothing else we can do
            // fetch store
            var storeRes = await _http.GetAsync($"{_catalogBaseUrl}/api/stores/{created.id}", ct);
            storeRes.EnsureSuccessStatusCode();
            store = await storeRes.Content.ReadFromJsonAsync<StoreDto>(cancellationToken: ct);
        }
        if (store is null) return;

        // 2) fetch existing products for the store
        var existing = await _http.GetFromJsonAsync<List<ProductDto>>($"{_catalogBaseUrl}/api/stores/{store.Id}/products", ct) ?? new List<ProductDto>();
        var existingNames = new HashSet<string>(existing.Select(p => p.Name), StringComparer.OrdinalIgnoreCase);

        // 3) create missing products
        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item.name)) continue;
            if (existingNames.Contains(item.name)) continue;
            var resp = await _http.PostAsJsonAsync($"{_catalogBaseUrl}/api/products", new { name = item.name, price = item.price, storeId = store.Id, categoryId = (Guid?)null }, ct);
            resp.EnsureSuccessStatusCode();
        }
    }

    private record StoreDto(Guid Id, string Name);
    private record ProductDto(Guid Id, string Name);
    private record CreatedResponse(Guid id);
}
