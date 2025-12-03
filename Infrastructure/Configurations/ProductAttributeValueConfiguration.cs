using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Configurations
{
    public sealed class ProductAttributeValueConfiguration : IEntityTypeConfiguration<ProductAttributeValue>
    {
        public void Configure(EntityTypeBuilder<ProductAttributeValue> b)
        {
            b.ToTable("product_attribute_values", "stroymag");

            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
                .ValueGeneratedNever();

            b.Property(x => x.ProductId)
                .IsRequired();

            b.Property(x => x.AttributeDefinitionId)
                .IsRequired();

            b.Property(x => x.StringValue)
                .HasMaxLength(1000);

            b.Property(x => x.NumericValue)
                .HasColumnType("numeric(18,3)");

            b.Property(x => x.BoolValue);

            b.HasIndex(x => new { x.ProductId, x.AttributeDefinitionId })
                .IsUnique();

            b.HasOne(x => x.Product)
                .WithMany(p => p.Attributes)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.AttributeDefinition)
                .WithMany(d => d.ProductValues)
                .HasForeignKey(x => x.AttributeDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
