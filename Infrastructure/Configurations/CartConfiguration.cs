using Domain.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Infrastructure.Configurations
{
    public sealed class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> b)
        {
            b.ToTable("carts", "stroymag");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).ValueGeneratedNever();

            b.Property(c => c.Items)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<CartItem>>(v, (JsonSerializerOptions?)null)
                         ?? new List<CartItem>(),
                    new ValueComparer<List<CartItem>>(
                        // Equals
                        (c1, c2) =>
                            ReferenceEquals(c1, c2)
                            || (c1 != null && c2 != null
                                && c1.Count == c2.Count
                                && c1.Zip(c2).All(x =>
                                    x.First.ProductId == x.Second.ProductId &&
                                    x.First.Name == x.Second.Name &&
                                    x.First.UnitPrice == x.Second.UnitPrice &&
                                    x.First.Qty == x.Second.Qty
                                )),

                        // GetHashCode
                        c => c == null
                            ? 0
                            : c.Aggregate(
                                0,
                                (a, v) => HashCode.Combine(
                                    a,
                                    v.ProductId,
                                    v.Name,
                                    v.UnitPrice,
                                    v.Qty
                                )),

                        // Snapshot
                        c => c == null
                            ? new List<CartItem>()
                            : c.Select(v => new CartItem(v.ProductId, v.Name, v.UnitPrice, v.Qty))
                                .ToList()
                    )
                )
                .HasColumnType("jsonb")
                .HasColumnName("items")
                .IsRequired();

            //b.Property(c => c.Items)
            //    .HasConversion(
            //        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            //        v => JsonSerializer.Deserialize<List<CartItem>>(v, (JsonSerializerOptions?)null),
            //        new ValueComparer<List<CartItem>>(
            //            (c1, c2) => c1.SequenceEqual(c2),
            //            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            //            c => c.ToList())
            //    )
            //    .HasColumnType("jsonb")
            //    .HasColumnName("items")
            //    .IsRequired();

            // PK=FK -> Customer
            b.HasOne<Customer>()
                .WithOne()
                .HasForeignKey<Cart>(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}