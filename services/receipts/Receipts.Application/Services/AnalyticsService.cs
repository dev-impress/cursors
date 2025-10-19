using Microsoft.EntityFrameworkCore;
using Receipts.Application.Ports;
using Receipts.Infrastructure.Db;

namespace Receipts.Application.Services;

public class AnalyticsService
{
    private readonly ReceiptsDbContext _db;
    private readonly ICatalogQueryService _catalogQuery;

    public AnalyticsService(ReceiptsDbContext db, ICatalogQueryService catalogQuery)
    {
        _db = db;
        _catalogQuery = catalogQuery;
    }

    // 1) Daily spend line chart for a date range
    public async Task<IReadOnlyList<(DateOnly day, decimal total)>> GetDailySpendAsync(DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        var fromDt = from.ToDateTime(TimeOnly.MinValue);
        var toDt = to.ToDateTime(TimeOnly.MaxValue);
        var groups = await _db.Receipts.AsNoTracking()
            .Where(r => r.IssuedAt >= fromDt && r.IssuedAt <= toDt)
            .GroupBy(r => DateOnly.FromDateTime(r.IssuedAt))
            .Select(g => new { Day = g.Key, Total = g.Sum(x => x.Total) })
            .OrderBy(x => x.Day)
            .ToListAsync(ct);
        return groups.Select(x => (x.Day, x.Total)).ToList();
    }

    // 2) Avg daily spend per month: returns list of (year, month, avgPerDay)
    public async Task<IReadOnlyList<(int year, int month, decimal avgPerDay)>> GetMonthlyAvgDailySpendAsync(int yearFrom, int monthFrom, int yearTo, int monthTo, CancellationToken ct = default)
    {
        var from = new DateTime(yearFrom, monthFrom, 1);
        var to = new DateTime(yearTo, monthTo, DateTime.DaysInMonth(yearTo, monthTo)).AddDays(1).AddTicks(-1);

        var data = await _db.Receipts.AsNoTracking()
            .Where(r => r.IssuedAt >= from && r.IssuedAt <= to)
            .GroupBy(r => new { r.IssuedAt.Year, r.IssuedAt.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Total = g.Sum(x => x.Total) })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToListAsync(ct);

        var result = new List<(int year, int month, decimal avgPerDay)>();
        foreach (var m in data)
        {
            var days = DateTime.DaysInMonth(m.Year, m.Month);
            var avg = days > 0 ? m.Total / days : 0m;
            result.Add((m.Year, m.Month, Math.Round(avg, 2)));
        }
        return result;
    }

    // 3) Current month spend share by category
    public async Task<IReadOnlyList<(string category, decimal total)>> GetCurrentMonthCategoryShareAsync(CancellationToken ct = default)
    {
        // We map receipts to stores by name equality and then to categories
        var now = DateTime.UtcNow;
        var first = new DateTime(now.Year, now.Month, 1);
        var last = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month)).AddDays(1).AddTicks(-1);

        var stores = await _catalogQuery.GetStoresAsync(ct);
        var nameToCategory = stores.ToDictionary(s => s.Name, s => s.CategoryName ?? "Uncategorized", StringComparer.OrdinalIgnoreCase);

        var data = await _db.Receipts.AsNoTracking()
            .Where(r => r.IssuedAt >= first && r.IssuedAt <= last)
            .ToListAsync(ct);

        var grouped = data.GroupBy(r => nameToCategory.TryGetValue(r.StoreName, out var cat) ? cat : "Uncategorized")
            .Select(g => new { Category = g.Key, Total = g.Sum(x => x.Total) })
            .OrderByDescending(x => x.Total)
            .ToList();

        return grouped.Select(x => (x.Category, x.Total)).ToList();
    }
}
