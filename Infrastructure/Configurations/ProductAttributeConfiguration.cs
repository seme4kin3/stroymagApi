using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public sealed class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductAttribute> b)
        {
            b.ToTable("product_attributes");
            b.HasKey(x => x.Id);
            b.Property(x => x.ProductId).HasMaxLength(64).IsRequired();
            b.Property(x => x.Key).HasMaxLength(100).IsRequired();
            b.Property(x => x.Value).HasMaxLength(1000).IsRequired();

            b.HasIndex(x => new { x.ProductId, x.Key }).IsUnique();

            b.HasOne<Product>()
             .WithMany(p => p.Attributes)
             .HasForeignKey(x => x.ProductId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
