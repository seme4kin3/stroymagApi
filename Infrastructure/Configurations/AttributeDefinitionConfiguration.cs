using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public sealed class AttributeDefinitionConfiguration : IEntityTypeConfiguration<AttributeDefinition>
    {
        public void Configure(EntityTypeBuilder<AttributeDefinition> b)
        {
            b.ToTable("attribute_definitions", "stroymag");

            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
                .ValueGeneratedNever();

            b.Property(x => x.Name)
                .HasMaxLength(300)
                .IsRequired();

            b.Property(x => x.Key)
                .HasMaxLength(100)
                .IsRequired();

            b.HasIndex(x => x.Key)
                .IsUnique();

            b.Property(x => x.DataType)
                .HasConversion<int>()
                .IsRequired();

            b.Property(x => x.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            b.HasMany(x => x.CategoryAttributes)
                .WithOne(ca => ca.AttributeDefinition)
                .HasForeignKey(ca => ca.AttributeDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(x => x.ProductValues)
                .WithOne(v => v.AttributeDefinition)
                .HasForeignKey(v => v.AttributeDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
