using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.Entities.ValueObjects;

namespace OrderManagementService.Infrastructure.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Ignore(x => x.DomainEvents);

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Price)
                .HasConversion(x => x.Value, v => Money.Create(v))
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Stock)
                .HasConversion(x => x.Value, v => Quantity.Create(v))
                .IsRequired();
        }
    }
}
