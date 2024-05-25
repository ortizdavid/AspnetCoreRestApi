using Microsoft.Extensions.DependencyInjection;
using AspNetCoreRestApi.Repositories;

namespace AspNetCoreRestApi.Extensions
{
    public static class RepositoryExtensions
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<CategoryRepository>();
            services.AddScoped<ProductRepository>();
            services.AddScoped<ImageRepository>();
            services.AddScoped<ProductReportRepository>();
            services.AddScoped<SupplierRepository>();
            services.AddScoped<UserRepository>();
        }
    }
}