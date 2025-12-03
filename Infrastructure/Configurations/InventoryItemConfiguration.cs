using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public sealed class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(EntityTypeBuilder<InventoryItem> b)
        {
            b.ToTable("inventory_items", "stroymag");

            // PK = FK на Product
            b.HasKey(x => x.ProductId);

            b.Property(x => x.ProductId)
                .ValueGeneratedNever();

            b.Property(x => x.Quantity)
                .HasColumnType("numeric(18,3)")
                .IsRequired();

            b.HasOne<Product>()
                .WithOne()
                .HasForeignKey<InventoryItem>(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
