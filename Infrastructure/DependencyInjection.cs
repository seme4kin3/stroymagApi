using Application.Abstractions;
using Application.Abstractions.Admin;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
        {
            var cs = cfg.GetConnectionString("Database")!;
            services.AddDbContext<StroymagDbContext>(opt =>
                opt.UseNpgsql(cs, npg =>
                {
                    npg.MigrationsHistoryTable("__ef_migrations_history", "stroymag");
                }));

            services.AddScoped<IProductReadRepository, ProductReadRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<IAttributeAdminRepository, AttributeAdminRepository>();
            services.AddScoped<ICategoryAdminRepository, CategoryAdminRepository>();
            services.AddScoped<IProductAdminRepository, ProductAdminRepository>();
            services.AddScoped<IBrandAdminRepository, BrandAdminRepository>();

            return services;
        }
    }
}
