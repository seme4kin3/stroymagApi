using Domain.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public sealed class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> b)
        {
            b.ToTable("carts");
            b.HasKey(x => x.Id);

            // Храним позиции корзины как jsonb (просто и надёжно)
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(c => c.Items)
             .HasConversion(
                 v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                 v => System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new())
             .HasColumnType("jsonb")
             .HasColumnName("items")
             .IsRequired();

            // PK=FK -> Customer
            b.HasOne<Customer>().WithOne().HasForeignKey<Cart>(x => x.Id).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
