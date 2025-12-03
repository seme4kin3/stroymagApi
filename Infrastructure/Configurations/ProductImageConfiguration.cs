using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public sealed class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> b)
        {
            b.ToTable("product_images", "stroymag");

            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
                .ValueGeneratedNever();

            b.Property(x => x.ProductId)
                .IsRequired();

            b.Property(x => x.Url)
                .HasMaxLength(500)
                .IsRequired();

            b.Property(x => x.StoragePath)
                .HasMaxLength(500)
                .IsRequired();

            b.Property(x => x.Alt)
                .HasMaxLength(500);

            b.Property(x => x.IsPrimary)
                .IsRequired();

            b.Property(x => x.SortOrder)
                .HasDefaultValue(0)
                .IsRequired();

            b.Property(x => x.CreatedAtUtc)
                .IsRequired();

            b.HasIndex(x => new { x.ProductId, x.SortOrder });

            b.HasOne<Product>()
                .WithMany(p => p.Images)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
