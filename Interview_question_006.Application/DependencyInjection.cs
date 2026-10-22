using Example.InterviewQuestion006.Application.Products;
using Microsoft.Extensions.DependencyInjection;

namespace Example.InterviewQuestion006.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ProductService>();
        return services;
    }
}
