using Domain.Catalog;
using Domain.Sales;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public sealed class StroymagDbContext : DbContext
    {
        public StroymagDbContext(DbContextOptions<StroymagDbContext> options) : base(options) { }

        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<AttributeDefinition> AttributeDefinitions => Set<AttributeDefinition>();
        public DbSet<CategoryAttribute> CategoryAttributes => Set<CategoryAttribute>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();
        public DbSet<InventoryItem> Inventory => Set<InventoryItem>();
        public DbSet<ProductAttributeValue> ProductAttributeValues => Set<ProductAttributeValue>();

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderLine> OrderLines => Set<OrderLine>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("stroymag");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StroymagDbContext).Assembly);

            //modelBuilder.ApplyConfiguration(new ProductConfiguration());
            //modelBuilder.ApplyConfiguration(new BrandConfiguration());
            //modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        }
    }
}
