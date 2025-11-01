using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public sealed class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(EntityTypeBuilder<InventoryItem> b)
        {
            b.ToTable("inventory");
            b.HasKey(x => x.ProductId);
            //b.Property(x => x.ProductId).HasMaxLength(64);
            b.Property(x => x.Quantity).HasColumnType("numeric(18,3)").HasDefaultValue(0);

            b.HasOne<Product>()
             .WithOne()
             .HasForeignKey<InventoryItem>(x => x.ProductId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
