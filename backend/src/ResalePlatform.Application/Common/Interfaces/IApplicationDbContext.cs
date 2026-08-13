using Microsoft.EntityFrameworkCore;
using ResalePlatform.Domain.Entities;

namespace ResalePlatform.Application.Common.Interfaces;

/// <summary>
/// Абстракция над EF Core DbContext для Application-слоя.
/// Открывает доступ только к доменным сущностям (без Identity-таблиц).
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<Listing> Listings { get; }
    DbSet<ListingImage> ListingImages { get; }
    DbSet<Favorite> Favorites { get; }
    DbSet<Conversation> Conversations { get; }
    DbSet<Message> Messages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
