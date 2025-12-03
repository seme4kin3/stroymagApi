using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Infrastructure.Configurations
{
    public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> b)
        {
            b.ToTable("products", "stroymag");

            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
                .ValueGeneratedNever();

            b.Property(x => x.Sku)
                .HasMaxLength(64)
                .IsRequired();

            b.Property(x => x.Article)
                .HasMaxLength(128)
                .IsRequired();

            b.Property(x => x.Name)
                .HasMaxLength(500)
                .IsRequired();

            b.Property(x => x.Description)
                .HasMaxLength(4000);

            b.Property(x => x.BrandId)
                .IsRequired();

            b.Property(x => x.CategoryId)
                .IsRequired();

            b.Property(x => x.UnitId)
                .IsRequired();

            b.Property(x => x.Price)
                .HasColumnType("numeric(18,2)")
                .IsRequired();

            b.Property(x => x.RecommendedRetailPrice)
                .HasColumnType("numeric(18,2)");

            b.Property(x => x.HasStock)
                .IsRequired();

            // ----- связи -----

            b.HasOne(x => x.Brand)
                .WithMany(br => br.Products)
                .HasForeignKey(x => x.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Unit)
                .WithMany(u => u.Products)
                .HasForeignKey(x => x.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(x => x.Images)
                .WithOne()
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.Attributes)
                .WithOne(v => v.Product)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            var listOfStringComparer = new ValueComparer<List<string>>(
                 (c1, c2) =>
                     ReferenceEquals(c1, c2) ||
                     (c1 != null && c2 != null && c1.SequenceEqual(c2)),
                 c => c == null
                     ? 0
                     : c.Aggregate(0, (a, v) => HashCode.Combine(a, v != null ? v.GetHashCode() : 0)),
                 c => c == null ? new List<string>() : c.ToList()
             );

            b.Property(p => p.Advantages)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)
                         ?? new List<string>(),
                    listOfStringComparer
                )
                .HasColumnType("jsonb")
                .HasColumnName("advantages")
                .IsRequired(false);


            b.Property(p => p.Complectation)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)
                         ?? new List<string>(),
                    listOfStringComparer
                )
                .HasColumnType("jsonb")
                .HasColumnName("complectation")
                .IsRequired(false);

        }
    }
}
