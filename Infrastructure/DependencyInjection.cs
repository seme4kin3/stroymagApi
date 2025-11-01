using Application.Abstractions;
using Infrastructure.Repositories;
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
                    // важно: история миграций в нашей схеме
                    npg.MigrationsHistoryTable("__ef_migrations_history", "stroymag");
                }));
            services.AddScoped<IProductReadRepository, ProductReadRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            return services;
        }
    }
}
