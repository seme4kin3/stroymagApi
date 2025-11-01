using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> b)
        {
            b.ToTable("products");

            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasMaxLength(64);

            b.Property(x => x.Sku).HasMaxLength(64).IsRequired();
            b.Property(x => x.Article).HasMaxLength(128).IsRequired();
            b.Property(x => x.Name).HasMaxLength(500).IsRequired();
            b.Property(x => x.Description).HasMaxLength(4000);

            b.Property(x => x.Price).HasColumnType("numeric(18,2)").IsRequired();
            b.Property(x => x.RecommendedRetailPrice)
                .HasColumnType("numeric(18,2)")
                .HasColumnName("Rrp")
                .IsRequired(false);

            b.Property(x => x.HasStock)
                .HasDefaultValue(true)
                .HasColumnName("Has_Stock");

            b.Property(x => x.BrandId).IsRequired();
            b.Property(x => x.CategoryId).IsRequired();

            b.HasIndex(x => x.Sku).IsUnique();
            b.HasIndex(x => x.Article);
            b.HasIndex(x => x.HasStock);

            b.HasOne(p => p.Brand)
                .WithMany(bd => bd.Products)
                .HasForeignKey(p => p.BrandId)
                .HasConstraintName("FK_products_brands_BrandId")
                .OnDelete(DeleteBehavior.Restrict);

            //b.HasOne<Brand>().WithMany().HasForeignKey(x => x.BrandId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<Category>()
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
