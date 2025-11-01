using Domain.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> b)
        {
            b.ToTable("customers");
            b.HasKey(x => x.Id);
            b.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            b.Property(x => x.Email).HasMaxLength(320).IsRequired();
            b.Property(x => x.Phone).HasMaxLength(32);

            b.HasIndex(x => x.Email).IsUnique();
        }
    }
}
