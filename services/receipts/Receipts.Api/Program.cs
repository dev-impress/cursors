using Microsoft.OpenApi.Models;
using Receipts.Application.Models;
using Receipts.Application.Services;
using Receipts.Infrastructure;
using Receipts.Infrastructure.Db;
using Receipts.Application.Ports;
using Receipts.Infrastructure.CatalogSync;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ReceiptsDb")
    ?? builder.Configuration["RECEIPTS_DB"]
    ?? "Host=localhost;Port=5432;Database=receipts;Username=postgres;Password=postgres";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Receipts API", Version = "v1" });
});

builder.Services.AddReceiptsInfrastructure(connectionString);

builder.Services.AddScoped<ReceiptService>();

builder.Services.AddHttpClient<ICatalogSyncService, CatalogSyncService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Ensure database is created (MVP)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReceiptsDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/api/receipts", async (ReceiptCreateRequest req, ReceiptService svc, ICatalogSyncService catalog, CancellationToken ct) =>
{
    // naive parse of items from payload: expects JSON like [{"name":"Milk","price":1.23}, ...]
    try
    {
        var items = System.Text.Json.JsonSerializer.Deserialize<List<Item>>(req.Payload) ?? new();
        var simplified = items.Where(i => !string.IsNullOrWhiteSpace(i.name)).Select(i => (i.name!, i.price)).ToList();
        if (simplified.Count > 0)
        {
            await catalog.EnsureStoreAndProductsAsync(req.StoreName, simplified, ct);
        }
    }
    catch
    {
        // ignore payload parsing errors; still save receipt
    }

    var id = await svc.CreateAsync(req, ct);
    return Results.Created($"/api/receipts/{id}", new { id });
}).WithOpenApi(op =>
{
    op.Summary = "Save a receipt; will create missing store/products in Catalog";
    return op;
});

app.MapGet("/api/receipts/{id:guid}", async (Guid id, ReceiptService svc, CancellationToken ct) =>
{
    var item = await svc.GetAsync(id, ct);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

app.MapGet("/api/receipts", async (int? skip, int? take, ReceiptService svc, CancellationToken ct) =>
{
    var items = await svc.ListAsync(skip ?? 0, take ?? 100, ct);
    return Results.Ok(items);
});

app.Run();

record Item(string? name, decimal price);
