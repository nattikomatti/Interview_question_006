using Example.InterviewQuestion006.Application.Barcodes;
using Example.InterviewQuestion006.Application.Products;
using Example.InterviewQuestion006.Infrastructure.Barcodes;
using Example.InterviewQuestion006.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Example.InterviewQuestion006.Infrastructure;

public static class DependencyInjection
{
    public const string ConnectionStringName = "DefaultConnection";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException($"Connection string '{ConnectionStringName}' is not configured.");

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddSingleton<IBarcodeGenerator, Code39BarcodeGenerator>();
        return services;
    }
}
