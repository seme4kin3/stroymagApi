using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Configurations
{
    public sealed class CategoryAttributeConfiguration : IEntityTypeConfiguration<CategoryAttribute>
    {
        public void Configure(EntityTypeBuilder<CategoryAttribute> b)
        {
            b.ToTable("category_attributes", "stroymag");

            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
                .ValueGeneratedNever();

            b.Property(x => x.CategoryId)
                .IsRequired();

            b.Property(x => x.AttributeDefinitionId)
                .IsRequired();

            b.Property(x => x.UnitId);

            b.Property(x => x.IsRequired)
                .IsRequired();

            b.Property(x => x.SortOrder)
                .HasDefaultValue(0)
                .IsRequired();

            b.HasIndex(x => new { x.CategoryId, x.AttributeDefinitionId })
                .IsUnique();

            b.HasOne(x => x.Category)
                .WithMany(c => c.CategoryAttributes)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.AttributeDefinition)
                .WithMany(d => d.CategoryAttributes)
                .HasForeignKey(x => x.AttributeDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Unit)
                .WithMany(u => u.CategoryAttributes)
                .HasForeignKey(x => x.UnitId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
