using Domain.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> b)
        {
            b.ToTable("orders");
            b.HasKey(x => x.Id);

            b.Property(x => x.Number).HasMaxLength(32).IsRequired();
            b.HasIndex(x => x.Number).IsUnique();

            b.Property(x => x.CustomerId).IsRequired();
            b.Property(x => x.Status).HasConversion<int>().IsRequired();

            b.Property(x => x.ShippingAddress).HasMaxLength(2000);
            b.Property(x => x.BillingAddress).HasMaxLength(2000);

            b.Property(x => x.Subtotal).HasColumnType("numeric(18,2)");
            b.Property(x => x.Discount).HasColumnType("numeric(18,2)");
            b.Property(x => x.Total).HasColumnType("numeric(18,2)");

            b.HasOne<Customer>().WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);

            b.HasMany(x => x.Lines).WithOne().HasForeignKey(l => l.OrderId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
