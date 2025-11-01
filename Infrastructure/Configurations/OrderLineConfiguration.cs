using Domain.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public sealed class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine>
    {
        public void Configure(EntityTypeBuilder<OrderLine> b)
        {
            b.ToTable("order_lines");
            b.HasKey(x => x.Id); // int identity

            b.Property(x => x.OrderId).IsRequired();
            b.Property(x => x.ProductId).HasMaxLength(64).IsRequired();
            b.Property(x => x.Name).HasMaxLength(500).IsRequired();

            b.Property(x => x.UnitPrice).HasColumnType("numeric(18,2)");
            b.Property(x => x.Qty).HasColumnType("numeric(18,3)");

            b.HasIndex(x => x.OrderId);
        }
    }
}
