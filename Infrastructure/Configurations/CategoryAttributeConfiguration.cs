using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
    public sealed class CategoryAttributeConfiguration : IEntityTypeConfiguration<CategoryAttribute>
    {
        public void Configure(EntityTypeBuilder<CategoryAttribute> b)
        {
            b.ToTable("category_attributes", "stroymag");

            b.HasKey(x => x.Id);

            b.Property(x => x.CategoryId)
                .IsRequired();

            b.Property(x => x.AttributeDefinitionId)
                .IsRequired();

            b.Property(x => x.IsRequired)
                .IsRequired();

            b.Property(x => x.SortOrder)
                .HasDefaultValue(0)
                .IsRequired();

            // один и тот же атрибут в категории — только один раз
            b.HasIndex(x => new { x.CategoryId, x.AttributeDefinitionId })
                .IsUnique();

            b.HasOne<Category>()
                .WithMany(c => c.Attributes)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<AttributeDefinition>()
                .WithMany()
                .HasForeignKey(x => x.AttributeDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
