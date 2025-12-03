using Domain.Catalog;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Configurations
{
    public class MeasurementUnitConfiguration : IEntityTypeConfiguration<MeasurementUnit>
    {
        public void Configure(EntityTypeBuilder<MeasurementUnit> b)
        {
            b.ToTable("measurement_units", "stroymag");

            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
                .ValueGeneratedNever();

            b.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            b.Property(x => x.Symbol)
                .HasMaxLength(32)
                .IsRequired();

            b.Property(x => x.Code)
                .HasMaxLength(32);

            b.Property(x => x.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            b.HasIndex(x => x.Symbol);

            b.HasMany(x => x.CategoryAttributes)
                .WithOne(ca => ca.Unit)
                .HasForeignKey(ca => ca.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(x => x.Products)
                .WithOne(p => p.Unit)
                .HasForeignKey(p => p.UnitId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
