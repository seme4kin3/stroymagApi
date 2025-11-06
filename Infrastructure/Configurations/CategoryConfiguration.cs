using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Configurations
{
    public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> b)
        {
            b.ToTable("categories");

            b.HasKey(x => x.Id);

            b.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            b.Property(x => x.ParentId);

            b.Property(x => x.Slug)
                .HasMaxLength(200);

            b.Property(x => x.ImageUrl)
                .HasMaxLength(500);

            // уникальность имени в рамках родителя
            b.HasIndex(x => new { x.ParentId, x.Name }).IsUnique();

            // полезно: быстрый поиск по slug
            b.HasIndex(x => x.Slug);

            //b.HasOne<Category>()
            //    .WithMany()
            //    .HasForeignKey(x => x.ParentId)
            //    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
