using Microsoft.OpenApi.Models;
using Receipts.Application.Models;
using Receipts.Application.Services;
using Receipts.Infrastructure;
using Receipts.Infrastructure.Db;

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

app.MapPost("/api/receipts", async (ReceiptCreateRequest req, ReceiptService svc, CancellationToken ct) =>
{
    var id = await svc.CreateAsync(req, ct);
    return Results.Created($"/api/receipts/{id}", new { id });
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
