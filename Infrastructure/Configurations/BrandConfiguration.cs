using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public sealed class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> b)
        {
            b.ToTable("brands", "stroymag");

            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
                .ValueGeneratedNever();

            b.Property(x => x.Name)
                .HasMaxLength(500)
                .IsRequired();

            b.HasIndex(x => x.Name)
                .IsUnique();

            b.HasMany(x => x.Products)
                .WithOne(p => p.Brand)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
