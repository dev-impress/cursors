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

        // 2) For each item: ensure a GlobalProduct exists; ensure StoreProduct exists; update price & history
        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item.name)) continue;

            // Global product by name
            var global = await _http.GetFromJsonAsync<GlobalProductDto?>($"{_catalogBaseUrl}/api/global-products/by-name/{Uri.EscapeDataString(item.name)}", ct);
            if (global is null)
            {
                var createGlobal = await _http.PostAsJsonAsync($"{_catalogBaseUrl}/api/global-products", new { name = item.name, categoryId = (Guid?)null }, ct);
                createGlobal.EnsureSuccessStatusCode();
                global = await createGlobal.Content.ReadFromJsonAsync<GlobalProductDto>(cancellationToken: ct);
            }
            if (global is null) continue;

            // Store product by store + storeSpecificName
            var storeProduct = await _http.GetFromJsonAsync<StoreProductDto?>($"{_catalogBaseUrl}/api/stores/{store.Id}/store-products/by-name/{Uri.EscapeDataString(item.name)}", ct);
            if (storeProduct is null)
            {
                var createSp = await _http.PostAsJsonAsync($"{_catalogBaseUrl}/api/stores/{store.Id}/store-products", new { globalProductId = global.Id, storeSpecificName = item.name, price = item.price }, ct);
                createSp.EnsureSuccessStatusCode();
            }
            else
            {
                // Update price (creates price history)
                var upd = await _http.PutAsJsonAsync($"{_catalogBaseUrl}/api/store-products/{storeProduct.Id}/price", new { price = item.price }, ct);
                upd.EnsureSuccessStatusCode();
            }
        }
    }

    private record StoreDto(Guid Id, string Name);
    private record CreatedResponse(Guid id);
    private record GlobalProductDto(Guid Id, string Name);
    private record StoreProductDto(Guid Id, string StoreSpecificName, decimal CurrentPrice);
}
