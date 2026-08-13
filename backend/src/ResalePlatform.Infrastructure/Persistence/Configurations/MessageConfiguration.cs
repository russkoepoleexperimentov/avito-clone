using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResalePlatform.Domain.Entities;

namespace ResalePlatform.Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Text).IsRequired().HasMaxLength(2000);

        builder.HasIndex(m => new { m.ConversationId, m.CreatedAt });
    }
}
