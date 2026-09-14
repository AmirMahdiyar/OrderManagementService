using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagementService.Infrastructure.Outbox;

namespace OrderManagementService.Infrastructure.Configurations
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages");

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                .HasMaxLength(250)
                .IsRequired();

            builder
                .Property(x => x.Content)
                .IsRequired();
        }
    }
}
