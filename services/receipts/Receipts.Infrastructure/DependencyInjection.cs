using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Receipts.Domain.Abstractions;
using Receipts.Infrastructure.Db;
using Receipts.Infrastructure.Repositories;

namespace Receipts.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddReceiptsInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ReceiptsDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IReceiptRepository, ReceiptRepository>();
        return services;
    }
}
