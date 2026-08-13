using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResalePlatform.Domain.Entities;

namespace ResalePlatform.Infrastructure.Persistence.Configurations;

public class ListingConfiguration : IEntityTypeConfiguration<Listing>
{
    public void Configure(EntityTypeBuilder<Listing> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Title).IsRequired().HasMaxLength(120);
        builder.Property(l => l.Description).IsRequired().HasMaxLength(5000);
        builder.Property(l => l.Price).HasColumnType("numeric(12,2)");
        builder.Property(l => l.City).IsRequired().HasMaxLength(80);

        // Enums хранятся как int (значение по умолчанию для EF), оставляем явно читаемым.
        builder.Property(l => l.Condition).HasConversion<int>();
        builder.Property(l => l.Status).HasConversion<int>();

        builder.HasIndex(l => l.CategoryId);
        builder.HasIndex(l => l.UserId);
        builder.HasIndex(l => l.Status);
        builder.HasIndex(l => l.CreatedAt);

        builder.HasOne(l => l.Category)
            .WithMany(c => c.Listings)
            .HasForeignKey(l => l.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(l => l.Images)
            .WithOne(i => i.Listing)
            .HasForeignKey(i => i.ListingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
