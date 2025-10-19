using Catalog.Application.Models;
using Catalog.Application.Services;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Db;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Catalog.Application.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("CatalogDb")
    ?? builder.Configuration["CATALOG_DB"]
    ?? "Host=localhost;Port=5432;Database=catalog;Username=postgres;Password=postgres";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog API", Version = "v1" });
});

builder.Services.AddCatalogInfrastructure(connectionString);

builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<StoreService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<GlobalProductService>();
builder.Services.AddScoped<StoreProductService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// Keycloak JWT auth
var authAuthority = builder.Configuration["Keycloak:Authority"] ?? builder.Configuration["AUTH_AUTHORITY"];
var authAudience = builder.Configuration["Keycloak:Audience"] ?? builder.Configuration["AUTH_AUDIENCE"] ?? "mvp-api";
if (!string.IsNullOrEmpty(authAuthority))
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = authAuthority;
            options.Audience = authAudience;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true
            };
        });
}

var app = builder.Build();

// Ensure database is created (MVP)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Categories
app.MapGet("/api/categories", async (CategoryService svc, CancellationToken ct) =>
{
    return Results.Ok(await svc.ListAsync(ct));
});

app.MapGet("/api/categories/{id:guid}", async (Guid id, CategoryService svc, CancellationToken ct) =>
{
    var item = await svc.GetAsync(id, ct);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

app.MapPost("/api/categories", async (CategoryCreateRequest req, CategoryService svc, CancellationToken ct) =>
{
    var id = await svc.CreateAsync(req, ct);
    return Results.Created($"/api/categories/{id}", new { id });
}).RequireAuthorization();

app.MapPut("/api/categories/{id:guid}", async (Guid id, CategoryUpdateRequest req, CategoryService svc, CancellationToken ct) =>
{
    var ok = await svc.UpdateAsync(id, req, ct);
    return ok ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

app.MapDelete("/api/categories/{id:guid}", async (Guid id, CategoryService svc, CancellationToken ct) =>
{
    var ok = await svc.DeleteAsync(id, ct);
    return ok ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

// Stores
app.MapGet("/api/stores", async (StoreService svc, CancellationToken ct) => Results.Ok(await svc.ListAsync(ct)));

app.MapGet("/api/stores/{id:guid}", async (Guid id, StoreService svc, CancellationToken ct) =>
{
    var item = await svc.GetAsync(id, ct);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

app.MapPost("/api/stores", async (StoreCreateRequest req, StoreService svc, CancellationToken ct) =>
{
    var id = await svc.CreateAsync(req, ct);
    return Results.Created($"/api/stores/{id}", new { id });
}).RequireAuthorization();

app.MapPut("/api/stores/{id:guid}", async (Guid id, StoreUpdateRequest req, StoreService svc, CancellationToken ct) =>
{
    var ok = await svc.UpdateAsync(id, req, ct);
    return ok ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

app.MapDelete("/api/stores/{id:guid}", async (Guid id, StoreService svc, CancellationToken ct) =>
{
    var ok = await svc.DeleteAsync(id, ct);
    return ok ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

// Products with effective category fallback (store category when product category is null)
app.MapGet("/api/products", async (ProductService svc, CancellationToken ct) => Results.Ok(await svc.ListAsync(ct)));

app.MapGet("/api/stores/{storeId:guid}/products", async (Guid storeId, ProductService svc, CancellationToken ct) =>
{
    return Results.Ok(await svc.ListByStoreAsync(storeId, ct));
});

app.MapGet("/api/products/{id:guid}", async (Guid id, ProductService svc, CancellationToken ct) =>
{
    var item = await svc.GetAsync(id, ct);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

app.MapPost("/api/products", async (ProductCreateRequest req, ProductService svc, CancellationToken ct) =>
{
    var id = await svc.CreateAsync(req, ct);
    return Results.Created($"/api/products/{id}", new { id });
}).RequireAuthorization();

app.MapPut("/api/products/{id:guid}", async (Guid id, ProductUpdateRequest req, ProductService svc, CancellationToken ct) =>
{
    var ok = await svc.UpdateAsync(id, req, ct);
    return ok ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

app.MapDelete("/api/products/{id:guid}", async (Guid id, ProductService svc, CancellationToken ct) =>
{
    var ok = await svc.DeleteAsync(id, ct);
    return ok ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

// Global products
app.MapGet("/api/global-products", async (GlobalProductService svc, CancellationToken ct) => Results.Ok(await svc.ListAsync(ct)));
app.MapGet("/api/global-products/{id:guid}", async (Guid id, GlobalProductService svc, CancellationToken ct) =>
{
    var item = await svc.GetDetailsAsync(id, ct);
    return item is null ? Results.NotFound() : Results.Ok(item);
});
app.MapGet("/api/global-products/by-name/{name}", async (string name, GlobalProductService svc, CancellationToken ct) =>
{
    var item = await svc.GetByNameAsync(name, ct);
    return item is null ? Results.NotFound() : Results.Ok(item);
});
app.MapPost("/api/global-products", async (GlobalProductCreateRequest req, GlobalProductService svc, CancellationToken ct) =>
{
    var id = await svc.CreateAsync(req, ct);
    return Results.Ok(new { id });
}).RequireAuthorization();
app.MapPut("/api/global-products/{id:guid}", async (Guid id, GlobalProductUpdateRequest req, GlobalProductService svc, CancellationToken ct) =>
{
    var ok = await svc.UpdateAsync(id, req, ct);
    return ok ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

// Store products (store-specific names and prices with history)
app.MapGet("/api/stores/{storeId:guid}/store-products/by-name/{name}", async (Guid storeId, string name, IStoreProductRepository repo, CancellationToken ct) =>
{
    var sp = await repo.GetByStoreAndNameAsync(storeId, name, ct);
    return sp is null ? Results.NotFound() : Results.Ok(new { Id = sp.Id, sp.StoreSpecificName, sp.CurrentPrice });
});
app.MapPost("/api/stores/{storeId:guid}/store-products", async (Guid storeId, StoreProductCreateRequest req, StoreProductService svc, CancellationToken ct) =>
{
    var id = await svc.CreateAsync(storeId, req, ct);
    return Results.Ok(new { id });
}).RequireAuthorization();
app.MapPut("/api/store-products/{id:guid}/price", async (Guid id, StoreProductPriceUpdateRequest req, StoreProductService svc, CancellationToken ct) =>
{
    var ok = await svc.UpdatePriceAsync(id, req, ct);
    return ok ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();
app.MapPut("/api/store-products/{id:guid}/rename", async (Guid id, StoreProductRenameRequest req, StoreProductService svc, CancellationToken ct) =>
{
    var ok = await svc.RenameAsync(id, req, ct);
    return ok ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

app.Run();
