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

            b.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            b.Property(x => x.Key)
                .HasMaxLength(100)
                .IsRequired();

            b.HasIndex(x => x.Key)
                .IsUnique();

            b.Property(x => x.DataType)
                .HasConversion<int>()        // enum -> int
                .IsRequired();

            b.Property(x => x.Unit)
                .HasMaxLength(50);

            b.Property(x => x.IsActive)
                .HasDefaultValue(true)
                .IsRequired();
        }
    }
}
