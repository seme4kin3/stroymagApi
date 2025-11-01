using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public sealed class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> b)
        {
            b.ToTable("product_images");
            b.HasKey(x => x.Id);
            b.Property(x => x.ProductId).HasMaxLength(64).IsRequired();
            b.Property(x => x.Url).HasMaxLength(1000).IsRequired();
            b.Property(x => x.StoragePath).HasMaxLength(1000).IsRequired();
            b.Property(x => x.Alt).HasMaxLength(500);
            b.Property(x => x.SortOrder).HasDefaultValue(0);
            b.Property(x => x.IsPrimary).HasDefaultValue(false);

            b.HasIndex(x => new { x.ProductId, x.IsPrimary });
            b.HasIndex(x => new { x.ProductId, x.SortOrder });

            b.HasOne<Product>()
             .WithMany(p => p.Images)
             .HasForeignKey(x => x.ProductId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
